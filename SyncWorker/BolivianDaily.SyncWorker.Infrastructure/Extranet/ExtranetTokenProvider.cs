using System.Net;
using System.Net.Http.Json;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;
using Microsoft.Extensions.Options;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet;

public sealed class ExtranetTokenProvider(HttpClient httpClient, IOptions<ExtranetOptions> options)
{
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private readonly TimeSpan _safetyMargin = TimeSpan.FromSeconds(30);

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt;

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (IsTokenValid())
        {
            return _accessToken!;
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            if (IsTokenValid())
            {
                return _accessToken!;
            }

            if (_refreshToken is not null)
            {
                try
                {
                    await RefreshTokenAsync(cancellationToken);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Invalidate();
                    await SignInAsync(cancellationToken);
                }
            }
            else
            {
                await SignInAsync(cancellationToken);
            }

            return _accessToken!;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public void Invalidate()
    {
        _accessToken = null;
        _refreshToken = null;
    }

    private bool IsTokenValid() =>
        _accessToken is not null
        && DateTime.UtcNow < _expiresAt - _safetyMargin;

    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        var extranetOptions = options.Value;
        var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/sign-in")
        {
            Content = JsonContent.Create(new SignInRequest(extranetOptions.Username, extranetOptions.Password))
        };

        await ApplyAuthAsync(request, cancellationToken);
    }

    private async Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh")
        {
            Content = JsonContent.Create(new RefreshRequest(_refreshToken))
        };
        request.Headers.Authorization = new("Bearer", _accessToken);

        await ApplyAuthAsync(request, cancellationToken);
    }

    private async Task ApplyAuthAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var wrapper = await response.Content.ReadFromJsonAsync<ExtranetApiResponse<ExtranetAuthData>>(cancellationToken);
        var data = wrapper?.Data
            ?? throw new InvalidOperationException("Extranet auth response did not contain data");

        _accessToken = data.AccessToken;
        _refreshToken = data.RefreshToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(options.Value.ExpiresInSeconds);
    }
}

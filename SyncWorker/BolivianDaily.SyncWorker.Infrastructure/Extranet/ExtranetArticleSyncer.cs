using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;
using BolivianDaily.SyncWorker.Infrastructure.Mappers;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet;

public sealed class ExtranetArticleSyncer(HttpClient httpClient, ExtranetTokenProvider tokenProvider) : IArticleSyncer
{
    public async Task<ArticleSyncResult> SyncAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        using var response = await PostArticleAsync(message, token, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            tokenProvider.Invalidate();
            token = await tokenProvider.GetTokenAsync(cancellationToken);
            using var retry = await PostArticleAsync(message, token, cancellationToken);
            return await ParseArticleAsync(retry, cancellationToken);
        }

        return await ParseArticleAsync(response, cancellationToken);
    }

    private async Task<HttpResponseMessage> PostArticleAsync(ArticleCheckedEvent message, string token, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/articles")
        {
            Content = JsonContent.Create(message.ToExtranetArticleRequest())
        };
        request.Headers.Authorization = new("Bearer", token);

        return await httpClient.SendAsync(request, cancellationToken);
    }

    private static async Task<ArticleSyncResult> ParseArticleAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw await CreateErrorAsync(response, cancellationToken);
        }

        var wrapper = await response.Content.ReadFromJsonAsync<ExtranetApiResponse<ExtranetArticleData>>(cancellationToken);
        var data = wrapper?.Data;

        return new ArticleSyncResult(data?.Id, null, data?.Status, wrapper?.Message);
    }

    private static async Task<ExtranetApiException> CreateErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var wrapper = await response.Content.ReadFromJsonAsync<ExtranetApiResponse<object>>(cancellationToken);
            if (!string.IsNullOrWhiteSpace(wrapper?.Message))
            {
                return new ExtranetApiException(wrapper.Message);
            }
        }
        catch (JsonException)
        {
        }

        return new ExtranetApiException($"Extranet request failed with status code {(int)response.StatusCode}");
    }
}

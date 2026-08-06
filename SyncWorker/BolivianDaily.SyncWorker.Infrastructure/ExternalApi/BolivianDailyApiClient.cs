using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace BolivianDaily.SyncWorker.Infrastructure.ExternalApi;

public sealed class BolivianDailyApiClient(HttpClient httpClient, IOptions<ExternalApiOptions> options) : IExternalNewsApiClient
{
    public async Task<string?> SendAsync(ArticleProcessedEvent message, CancellationToken cancellationToken = default)
    {
        var externalApiOptions = options.Value;
        if (string.IsNullOrWhiteSpace(externalApiOptions.Url))
        {
            throw new InvalidOperationException("ExternalApi:Url is not configured");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, externalApiOptions.Url);
        request.Headers.Add("X-API-TOKEN", externalApiOptions.Token);
        request.Content = JsonContent.Create(new ExternalArticleRequest(
            message.CategoryId,
            message.SourceId,
            1,
            message.Title,
            message.Pretitle,
            message.Subtitle,
            message.Enter,
            message.Body,
            message.Author,
            message.PublicationDate,
            message.State,
            message.Media.Select(media => new ExternalArticleMediaRequest(media.Url, media.Type)).ToArray()));

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return response.Headers.Location?.ToString();
    }

    private sealed record ExternalArticleRequest(
        [property: JsonPropertyName("category_id")] long? CategoryId,
        [property: JsonPropertyName("source_id")] long? SourceId,
        [property: JsonPropertyName("user_id")] int UserId,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("pretitle")] string? Pretitle,
        [property: JsonPropertyName("subtitle")] string? Subtitle,
        [property: JsonPropertyName("enter")] string? Enter,
        [property: JsonPropertyName("body")] string Body,
        [property: JsonPropertyName("author")] string? Author,
        [property: JsonPropertyName("publication_date")] DateTime? PublicationDate,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("multimedia")] IReadOnlyCollection<ExternalArticleMediaRequest> Multimedia);

    private sealed record ExternalArticleMediaRequest(
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("type")] string Type);
}

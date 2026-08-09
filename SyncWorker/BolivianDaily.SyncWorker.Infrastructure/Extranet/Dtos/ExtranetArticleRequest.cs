using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetArticleRequest(
    [property: JsonPropertyName("category_id")]
    long? CategoryId,
    [property: JsonPropertyName("source_id")]
    long? SourceId,
    [property: JsonPropertyName("url")]
    string? Url,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("pretitle")]
    string? Pretitle,
    [property: JsonPropertyName("subtitle")]
    string? Subtitle,
    [property: JsonPropertyName("lead")]
    string? Lead,
    [property: JsonPropertyName("body")]
    string Body,
    [property: JsonPropertyName("author")]
    string? Author,
    [property: JsonPropertyName("published_at")]
    DateTime? PublishedAt,
    [property: JsonPropertyName("media")]
    IReadOnlyCollection<ExtranetArticleMediaRequest> Media
);

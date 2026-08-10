using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetArticleData(
    [property: JsonPropertyName("id")]
    long Id,
    [property: JsonPropertyName("url")]
    string? Url,
    [property: JsonPropertyName("status")]
    string? Status
);

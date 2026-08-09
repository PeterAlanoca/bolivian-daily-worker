using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetArticleData(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("status")]
    string? Status
);

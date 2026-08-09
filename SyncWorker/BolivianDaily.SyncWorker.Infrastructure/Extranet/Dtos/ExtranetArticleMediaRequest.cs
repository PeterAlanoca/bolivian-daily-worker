using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetArticleMediaRequest(
    [property: JsonPropertyName("url")]
    string Url,
    [property: JsonPropertyName("type")]
    string Type
);

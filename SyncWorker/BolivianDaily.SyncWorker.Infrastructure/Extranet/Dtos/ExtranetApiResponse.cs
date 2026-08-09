using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetApiResponse<T>(
    [property: JsonPropertyName("success")]
    bool Success,
    [property: JsonPropertyName("data")]
    T? Data,
    [property: JsonPropertyName("message")]
    string? Message
);

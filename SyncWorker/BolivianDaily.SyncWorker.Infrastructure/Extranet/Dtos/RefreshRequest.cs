using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record RefreshRequest(
    [property: JsonPropertyName("refresh_token")]
    string? RefreshToken
);

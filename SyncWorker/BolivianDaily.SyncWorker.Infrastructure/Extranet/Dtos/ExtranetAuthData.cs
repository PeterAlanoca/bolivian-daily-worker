using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record ExtranetAuthData(
    [property: JsonPropertyName("access_token")]
    string AccessToken,
    [property: JsonPropertyName("refresh_token")]
    string RefreshToken,
    [property: JsonPropertyName("expires_in")]
    int ExpiresIn
);

using System.Text.Json.Serialization;

namespace BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

public sealed record SignInRequest(
    [property: JsonPropertyName("username")]
    string Username,
    [property: JsonPropertyName("password")]
    string Password
);

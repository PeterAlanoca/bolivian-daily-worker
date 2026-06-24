using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record OpenRouterMessage(
    [property: JsonPropertyName("role")]
    string Role,
    [property: JsonPropertyName("content")]
    string Content
);

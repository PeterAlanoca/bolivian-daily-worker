using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record OpenRouterChoice(
    [property: JsonPropertyName("index")]
    int Index,
    [property: JsonPropertyName("finish_reason")]
    string FinishReason,
    [property: JsonPropertyName("message")]
    OpenRouterMessage Message
);

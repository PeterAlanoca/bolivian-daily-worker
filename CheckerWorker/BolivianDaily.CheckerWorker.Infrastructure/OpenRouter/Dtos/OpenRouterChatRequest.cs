using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record OpenRouterChatRequest(
    [property: JsonPropertyName("model")]
    string Model,
    [property: JsonPropertyName("temperature")]
    float Temperature,
    [property: JsonPropertyName("messages")]
    IReadOnlyCollection<OpenRouterMessage> Messages
);

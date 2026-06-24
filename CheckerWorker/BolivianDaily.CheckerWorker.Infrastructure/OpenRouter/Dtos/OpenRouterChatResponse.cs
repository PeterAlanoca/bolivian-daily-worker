using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record OpenRouterChatResponse(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("object")]
    string Object,
    [property: JsonPropertyName("created")]
    long Created,
    [property: JsonPropertyName("model")]
    string Model,
    [property: JsonPropertyName("choices")]
    IReadOnlyCollection<OpenRouterChoice> Choices,
    [property: JsonPropertyName("usage")]
    OpenRouterUsage? Usage
);

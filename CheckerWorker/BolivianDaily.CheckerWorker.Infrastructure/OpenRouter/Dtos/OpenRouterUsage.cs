using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record OpenRouterUsage(
    [property: JsonPropertyName("prompt_tokens")]
    int PromptTokens,
    [property: JsonPropertyName("completion_tokens")]
    int CompletionTokens,
    [property: JsonPropertyName("total_tokens")]
    int TotalTokens,
    [property: JsonPropertyName("cost")]
    decimal Cost
);

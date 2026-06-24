using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record CategoryAccuracyValidation(
    [property: JsonPropertyName("passed")]
    bool Passed,
    [property: JsonPropertyName("reason")]
    string Reason,
    [property: JsonPropertyName("suggestedCategory")]
    string? SuggestedCategory
);

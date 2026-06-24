using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record NoAdvertisingValidation(
    [property: JsonPropertyName("passed")]
    bool Passed,
    [property: JsonPropertyName("reason")]
    string Reason,
    [property: JsonPropertyName("detectedNetworks")]
    IReadOnlyCollection<string> DetectedNetworks
);

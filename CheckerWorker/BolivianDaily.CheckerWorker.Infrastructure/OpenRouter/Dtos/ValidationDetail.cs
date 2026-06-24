using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record ValidationDetail(
    [property: JsonPropertyName("passed")]
    bool Passed,
    [property: JsonPropertyName("reason")]
    string Reason
);

using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record ArticleValidations(
    [property: JsonPropertyName("htmlFormat")]
    ValidationDetail HtmlFormat,
    [property: JsonPropertyName("categoryAccuracy")]
    CategoryAccuracyValidation CategoryAccuracy,
    [property: JsonPropertyName("noAdvertising")]
    NoAdvertisingValidation NoAdvertising,
    [property: JsonPropertyName("readyToPublish")]
    ValidationDetail ReadyToPublish
);

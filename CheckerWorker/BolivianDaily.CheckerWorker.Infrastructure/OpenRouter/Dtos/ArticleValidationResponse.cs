using System.Text.Json.Serialization;

namespace BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;

public sealed record ArticleValidationResponse(
    [property: JsonPropertyName("articleId")]
    long ArticleId,
    [property: JsonPropertyName("isValid")]
    bool IsValid,
    [property: JsonPropertyName("validations")]
    ArticleValidations Validations
);

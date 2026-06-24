using System.Text.Json;
using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Infrastructure.Mappers;

public static class OpenRouterMapper
{
    public static ProcessedArticle ToProcessedArticle(this ArticleValidationResponse validation, ArticleScrapedEvent source)
    {
        return new ProcessedArticle
        {
            ScrapedArticleId = source.ArticleId,
            SourceId = source.SourceId,
            CategoryId = source.CategoryId,
            Title = source.Title,
            Pretitle = source.Pretitle,
            Subtitle = source.Subtitle,
            Enter = source.Lead,
            Body = source.RawBody ?? source.Lead ?? source.Title,
            Author = source.Author,
            PublicationDate = source.PublicationDate,
            Multimedia = [.. source.Multimedia.Select(MapMedia)],
            Warnings = null,
            IsValid = validation.IsValid,
            HtmlFormatPassed = validation.Validations.HtmlFormat.Passed,
            HtmlFormatReason = validation.Validations.HtmlFormat.Reason,
            CategoryAccuracyPassed = validation.Validations.CategoryAccuracy.Passed,
            CategoryAccuracyReason = validation.Validations.CategoryAccuracy.Reason,
            SuggestedCategory = validation.Validations.CategoryAccuracy.SuggestedCategory,
            NoAdvertisingPassed = validation.Validations.NoAdvertising.Passed,
            NoAdvertisingReason = validation.Validations.NoAdvertising.Reason,
            DetectedNetworksJson = validation.Validations.NoAdvertising.DetectedNetworks.Count > 0
                ? JsonSerializer.Serialize(validation.Validations.NoAdvertising.DetectedNetworks)
                : null,
            ReadyToPublishPassed = validation.Validations.ReadyToPublish.Passed,
            ReadyToPublishReason = validation.Validations.ReadyToPublish.Reason
        };
    }

    public static ProcessedArticle ToProcessedArticle(this ArticleScrapedEvent source, string warning)
    {
        return new ProcessedArticle
        {
            ScrapedArticleId = source.ArticleId,
            SourceId = source.SourceId,
            CategoryId = source.CategoryId,
            Title = source.Title,
            Pretitle = source.Pretitle,
            Subtitle = source.Subtitle,
            Enter = source.Lead,
            Body = source.RawBody ?? source.Lead ?? source.Title,
            Author = source.Author,
            PublicationDate = source.PublicationDate,
            Multimedia = [.. source.Multimedia.Select(MapMedia)],
            Warnings = warning,
            IsValid = false
        };
    }

    public static OpenRouterChatRequest ToOpenRouterChatRequest(this ArticleScrapedEvent source, string model, string prompt)
    {
        return new OpenRouterChatRequest(
            Model: model,
            Temperature: 0,
            Messages:
            [
                new OpenRouterMessage("system", prompt),
                new OpenRouterMessage("user", JsonSerializer.Serialize(source))
            ]
        );
    }

    private static ProcessedArticleMedia MapMedia(ArticleMediaMessage media)
    {
        return new ProcessedArticleMedia
        {
            Url = media.Url,
            Type = media.Type,
            Description = media.Description,
            Path = media.Path
        };
    }

}

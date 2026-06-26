using System.Text.Json;
using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.CheckerWorker.Infrastructure.OpenRouter.Dtos;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Infrastructure.Mappers;

public static class OpenRouterMapper
{
    public static ProcessedArticle ToProcessedArticle(this ArticleValidationResponse articleValidationResponse, ArticleScrapedEvent articleScrapedEvent)
    {
        return new ProcessedArticle
        {
            ScrapedArticleId = articleScrapedEvent.ArticleId,
            SourceId = articleScrapedEvent.SourceId,
            SourceName = articleScrapedEvent.SourceName,
            SourceUrl = articleScrapedEvent.SourceUrl,
            ArticleUrl = articleScrapedEvent.ArticleUrl ?? string.Empty,
            CategoryId = articleScrapedEvent.CategoryId,
            CategoryName = articleScrapedEvent.CategoryName,
            Title = articleScrapedEvent.Title,
            Pretitle = articleScrapedEvent.Pretitle,
            Subtitle = articleScrapedEvent.Subtitle,
            Enter = articleScrapedEvent.Lead,
            Body = articleScrapedEvent.RawBody ?? articleScrapedEvent.Lead ?? articleScrapedEvent.Title,
            Author = articleScrapedEvent.Author,
            PublicationDate = articleScrapedEvent.PublicationDate,
            ScrapedAt = articleScrapedEvent.ScrapedAt,
            Media = [.. articleScrapedEvent.Media.Select(MapMedia)],
            Warnings = null,
            IsValid = articleValidationResponse.IsValid,
            HtmlFormatPassed = articleValidationResponse.Validations.HtmlFormat.Passed,
            HtmlFormatReason = articleValidationResponse.Validations.HtmlFormat.Reason,
            CategoryAccuracyPassed = articleValidationResponse.Validations.CategoryAccuracy.Passed,
            CategoryAccuracyReason = articleValidationResponse.Validations.CategoryAccuracy.Reason,
            SuggestedCategory = articleValidationResponse.Validations.CategoryAccuracy.SuggestedCategory,
            NoAdvertisingPassed = articleValidationResponse.Validations.NoAdvertising.Passed,
            NoAdvertisingReason = articleValidationResponse.Validations.NoAdvertising.Reason,
            DetectedNetworks = articleValidationResponse.Validations.NoAdvertising.DetectedNetworks.Count > 0
                ? JsonSerializer.Serialize(articleValidationResponse.Validations.NoAdvertising.DetectedNetworks)
                : null,
            ReadyToPublishPassed = articleValidationResponse.Validations.ReadyToPublish.Passed,
            ReadyToPublishReason = articleValidationResponse.Validations.ReadyToPublish.Reason
        };
    }

    public static ProcessedArticle ToProcessedArticle(this ArticleScrapedEvent articleScrapedEvent, string warning)
    {
        return new ProcessedArticle
        {
            ScrapedArticleId = articleScrapedEvent.ArticleId,
            SourceId = articleScrapedEvent.SourceId,
            SourceName = articleScrapedEvent.SourceName,
            SourceUrl = articleScrapedEvent.SourceUrl,
            ArticleUrl = articleScrapedEvent.ArticleUrl ?? string.Empty,
            CategoryId = articleScrapedEvent.CategoryId,
            CategoryName = articleScrapedEvent.CategoryName,
            Title = articleScrapedEvent.Title,
            Pretitle = articleScrapedEvent.Pretitle,
            Subtitle = articleScrapedEvent.Subtitle,
            Enter = articleScrapedEvent.Lead,
            Body = articleScrapedEvent.RawBody ?? articleScrapedEvent.Lead ?? articleScrapedEvent.Title,
            Author = articleScrapedEvent.Author,
            PublicationDate = articleScrapedEvent.PublicationDate,
            ScrapedAt = articleScrapedEvent.ScrapedAt,
            Media = [.. articleScrapedEvent.Media.Select(MapMedia)],
            Warnings = warning,
            IsValid = false
        };
    }

    public static OpenRouterChatRequest ToOpenRouterChatRequest(this ArticleScrapedEvent articleScrapedEvent, string model, string prompt)
    {
        return new OpenRouterChatRequest(
            Model: model,
            Temperature: 0,
            Messages:
            [
                new OpenRouterMessage("system", prompt),
                new OpenRouterMessage("user", JsonSerializer.Serialize(articleScrapedEvent))
            ]
        );
    }

    private static ProcessedArticleMedia MapMedia(ArticleMediaMessage articleMediaMessage)
    {
        return new ProcessedArticleMedia
        {
            Url = articleMediaMessage.Url,
            Type = articleMediaMessage.Type,
            Description = articleMediaMessage.Description,
            Path = articleMediaMessage.Path
        };
    }

}

using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Mappers;

public static class ProcessedArticleMappers
{
    public static ArticleProcessedEvent AsEvent(this ProcessedArticle article)
    {
        return new ArticleProcessedEvent(
            ScrapedArticleId: article.ScrapedArticleId,
            ProcessedArticleId: article.Id,
            CategoryId: article.CategoryId,
            SourceId: article.SourceId,
            SourceName: article.SourceName,
            SourceUrl: article.SourceUrl,
            ArticleUrl: article.ArticleUrl,
            CategoryName: article.CategoryName,
            Title: article.Title,
            Pretitle: article.Pretitle,
            Subtitle: article.Subtitle,
            Enter: article.Enter,
            Body: article.Body,
            Author: article.Author,
            PublicationDate: article.PublicationDate,
            ScrapedAt: article.ScrapedAt,
            State: article.State,
            Multimedia: [.. article.Multimedia.Select(media => new ArticleMediaMessage(
                media.Url,
                media.Type,
                media.Description,
                media.Path))],
            ProcessedAt: article.ProcessedAt);
    }
}

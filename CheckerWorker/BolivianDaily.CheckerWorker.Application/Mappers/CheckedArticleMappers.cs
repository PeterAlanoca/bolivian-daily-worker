using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Mappers;

public static class CheckedArticleMappers
{
    public static ArticleCheckedEvent ToCheckedEvent(this CheckedArticle article)
    {
        return new ArticleCheckedEvent(
            ScrapedArticleId: article.ScrapedArticleId,
            CheckedArticleId: article.Id,
            CategoryId: article.CategoryId,
            SourceId: article.SourceId,
            SourceName: article.SourceName,
            SourceUrl: article.SourceUrl,
            Url: article.Url,
            CategoryName: article.CategoryName,
            Title: article.Title,
            Pretitle: article.Pretitle,
            Subtitle: article.Subtitle,
            Lead: article.Lead,
            Body: article.Body,
            Author: article.Author,
            PublishedAt: article.PublishedAt,
            ScrapedAt: article.ScrapedAt,
            State: article.State,
            Media: [.. article.Media.Select(media => new ArticleMediaMessage(
                media.Url,
                media.Type,
                media.Description,
                media.Path))],
            CheckedAt: article.CheckedAt);
    }
}

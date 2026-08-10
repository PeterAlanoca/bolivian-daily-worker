using BolivianDaily.CheckerWorker.Domain.Entities;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Mappers;

public static class CheckedArticleMappers
{
    public static ArticleCheckedEvent ToCheckedEvent(this CheckedArticle checkedArticle)
    {
        return new ArticleCheckedEvent(
            ScrapedArticleId: checkedArticle.ScrapedArticleId,
            CheckedArticleId: checkedArticle.Id,
            CategoryId: checkedArticle.CategoryId,
            SourceId: checkedArticle.SourceId,
            SourceName: checkedArticle.SourceName,
            SourceUrl: checkedArticle.SourceUrl,
            Url: checkedArticle.Url,
            CategoryName: checkedArticle.CategoryName,
            Title: checkedArticle.Title,
            Pretitle: checkedArticle.Pretitle,
            Subtitle: checkedArticle.Subtitle,
            Lead: checkedArticle.Lead,
            Body: checkedArticle.Body,
            Author: checkedArticle.Author,
            PublishedAt: checkedArticle.PublishedAt,
            ScrapedAt: checkedArticle.ScrapedAt,
            Media: [.. checkedArticle.Media.Select(media => new ArticleMediaMessage(
                media.Url,
                media.Type,
                media.Description,
                media.Path))],
            CheckedAt: checkedArticle.CheckedAt);
    }
}

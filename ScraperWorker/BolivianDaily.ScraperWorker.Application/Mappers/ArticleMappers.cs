using BolivianDaily.Shared.Messaging;
using BolivianDaily.ScraperWorker.Domain.Entities;

namespace BolivianDaily.ScraperWorker.Application.Mappers;

public static class ArticleMappers
{
    public static ArticleScrapedEvent AsScrapedEvent(this Article article, NewsSource source, SourceCategory sourceCategory)
    {
        return new ArticleScrapedEvent(
            ArticleId: article.Id,
            SourceId: source.Id,
            SourceName: source.Name,
            SourceUrl: source.BaseUrl,
            CategoryId: sourceCategory.CategoryId,
            CategoryName: sourceCategory.Name,
            Pretitle: article.Pretitle,
            Title: article.Title,
            Subtitle: article.Subtitle,
            Lead: article.Lead,
            RawBody: article.Body,
            Author: article.Author,
            PublicationDate: article.PublishedAt,
            Multimedia: article.Media
                .Select(m => new ArticleMediaMessage(m.Url, m.Type, m.Description, m.Path))
                .ToList(),
            ScrapedAt: article.ScrapedAt);
    }
}

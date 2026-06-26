using BolivianDaily.Shared.Messaging;
using BolivianDaily.ScraperWorker.Domain.Entities;

namespace BolivianDaily.ScraperWorker.Application.Mappers;

public static class ArticleMappers
{
    public static ArticleScrapedEvent AsScrapedEvent(this Article article, NewsSource source, Category category)
    {
        return new ArticleScrapedEvent(
            ArticleId: article.Id,
            SourceId: source.Id,
            SourceName: source.Name,
            SourceUrl: source.BaseUrl,
            ArticleUrl: article.Url,
            CategoryId: category.Id,
            CategoryName: category.Name,
            Pretitle: article.Pretitle,
            Title: article.Title,
            Subtitle: article.Subtitle,
            Lead: article.Lead,
            RawBody: article.Body,
            Author: article.Author,
            PublicationDate: article.PublishedAt,
            Media: [.. article.Media.Select(m => new ArticleMediaMessage(m.Url, m.Type, m.Description, m.Path))],
            ScrapedAt: article.ScrapedAt);
    }
}

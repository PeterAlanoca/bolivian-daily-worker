using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Domain.Entities;

namespace BolivianDaily.SyncWorker.Application.Mappers;

public static class SyncedArticleMappers
{
    public static SyncedArticle ToSyncedArticle(this ArticleCheckedEvent articleCheckedEvent)
    {
        return new SyncedArticle
        {
            CheckedArticleId = articleCheckedEvent.CheckedArticleId,
            ScrapedArticleId = articleCheckedEvent.ScrapedArticleId,
            SourceId = articleCheckedEvent.SourceId,
            SourceName = articleCheckedEvent.SourceName,
            SourceUrl = articleCheckedEvent.SourceUrl,
            Url = articleCheckedEvent.Url ?? string.Empty,
            CategoryId = articleCheckedEvent.CategoryId,
            CategoryName = articleCheckedEvent.CategoryName,
            Title = articleCheckedEvent.Title,
            Pretitle = articleCheckedEvent.Pretitle,
            Subtitle = articleCheckedEvent.Subtitle,
            Lead = articleCheckedEvent.Lead,
            Body = articleCheckedEvent.Body,
            Author = articleCheckedEvent.Author,
            PublishedAt = articleCheckedEvent.PublishedAt,
            ScrapedAt = articleCheckedEvent.ScrapedAt,
            CheckedAt = articleCheckedEvent.CheckedAt,
            Attempts = 1,
            Media = [.. articleCheckedEvent.Media.Select(media => new SyncedArticleMedia
            {
                Url = media.Url,
                Type = media.Type,
                Description = media.Description,
                Path = media.Path
            })]
        };
    }
}

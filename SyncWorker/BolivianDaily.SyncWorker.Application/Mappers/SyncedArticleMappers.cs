using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Domain.Entities;

namespace BolivianDaily.SyncWorker.Application.Mappers;

public static class SyncedArticleMappers
{
    public static SyncedArticle ToSyncedArticle(this ArticleCheckedEvent message)
    {
        return new SyncedArticle
        {
            CheckedArticleId = message.CheckedArticleId,
            ScrapedArticleId = message.ScrapedArticleId,
            SourceId = message.SourceId,
            SourceName = message.SourceName,
            SourceUrl = message.SourceUrl,
            Url = message.Url ?? string.Empty,
            CategoryId = message.CategoryId,
            CategoryName = message.CategoryName,
            Title = message.Title,
            Pretitle = message.Pretitle,
            Subtitle = message.Subtitle,
            Lead = message.Lead,
            Body = message.Body,
            Author = message.Author,
            PublishedAt = message.PublishedAt,
            ScrapedAt = message.ScrapedAt,
            CheckedAt = message.CheckedAt,
            Attempts = 1,
            Media = [.. message.Media.Select(media => new SyncedArticleMedia
            {
                Url = media.Url,
                Type = media.Type,
                Description = media.Description,
                Path = media.Path
            })]
        };
    }
}

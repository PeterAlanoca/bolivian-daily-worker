using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Infrastructure.Extranet.Dtos;

namespace BolivianDaily.SyncWorker.Infrastructure.Mappers;

public static class ExtranetArticleMappers
{
    public static ExtranetArticleRequest ToExtranetArticleRequest(this ArticleCheckedEvent articleCheckedEvent)
    {
        return new ExtranetArticleRequest(
            articleCheckedEvent.CategoryId,
            articleCheckedEvent.SourceId,
            articleCheckedEvent.Url,
            articleCheckedEvent.Title,
            articleCheckedEvent.Pretitle,
            articleCheckedEvent.Subtitle,
            articleCheckedEvent.Lead,
            articleCheckedEvent.Body,
            articleCheckedEvent.Author,
            articleCheckedEvent.PublishedAt,
            articleCheckedEvent.Media
                .Select(media => new ExtranetArticleMediaRequest(media.Url, media.Type))
                .ToArray());
    }
}

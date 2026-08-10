using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.ScraperWorker.Application.Interfaces;

public interface IArticleScrapedEventPublisher
{
    Task PublishAsync(ArticleScrapedEvent articleScrapedEvent, CancellationToken cancellationToken = default);
}

using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.ScraperWorker.Application.Interfaces;

public interface IArticleScrapedEventPublisher
{
    Task PublishAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default);
}

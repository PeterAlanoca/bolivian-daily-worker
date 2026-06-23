using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Interfaces;

public interface IArticleProcessedEventPublisher
{
    Task PublishAsync(ArticleProcessedEvent message, CancellationToken cancellationToken = default);
}

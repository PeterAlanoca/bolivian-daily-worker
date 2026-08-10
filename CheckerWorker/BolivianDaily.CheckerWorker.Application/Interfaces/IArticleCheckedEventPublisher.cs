using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.CheckerWorker.Application.Interfaces;

public interface IArticleCheckedEventPublisher
{
    Task PublishAsync(ArticleCheckedEvent articleCheckedEvent, CancellationToken cancellationToken = default);
}

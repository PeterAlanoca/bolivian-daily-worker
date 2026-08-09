using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.SyncWorker.Application.Interfaces;

public interface IExternalNewsApiClient
{
    Task<CloudArticleResult> SendAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default);
}

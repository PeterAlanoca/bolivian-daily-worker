using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.SyncWorker.Application.Interfaces;

public interface IArticleSyncer
{
    Task<ArticleSyncResult> SyncAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default);
}

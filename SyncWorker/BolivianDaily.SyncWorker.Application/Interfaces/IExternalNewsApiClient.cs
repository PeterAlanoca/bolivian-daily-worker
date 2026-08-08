using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.SyncWorker.Application.Interfaces;

public interface IExternalNewsApiClient
{
    Task<string?> SendAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default);
}

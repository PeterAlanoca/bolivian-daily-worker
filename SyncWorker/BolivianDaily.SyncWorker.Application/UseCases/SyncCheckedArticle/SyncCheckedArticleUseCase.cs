using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Application.Mappers;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;

public sealed class SyncCheckedArticleUseCase(
    IExternalNewsApiClient apiClient,
    ISyncedArticleRepository repository,
    ILogger<SyncCheckedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default)
    {
        if (await repository.IsSyncedAsync(message.CheckedArticleId, cancellationToken))
        {
            logger.LogInformation("Checked article {CheckedArticleId} was already synced", message.CheckedArticleId);
            return;
        }

        var syncedArticle = message.ToSyncedArticle();

        try
        {
            var result = await apiClient.SendAsync(message, cancellationToken);
            syncedArticle.CloudId = result.Id;
            syncedArticle.CloudUrl = result.Url;
            syncedArticle.Status = "SYNCED";
            syncedArticle.SyncedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            syncedArticle.Status = "FAILED";
            syncedArticle.Details = ex.Message;
            await repository.AddAsync(syncedArticle, cancellationToken);
            throw;
        }

        await repository.AddAsync(syncedArticle, cancellationToken);
        logger.LogInformation("Synced checked article {CheckedArticleId}", message.CheckedArticleId);
    }
}

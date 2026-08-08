using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Domain.Entities;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;

public sealed class SyncCheckedArticleUseCase(
    IExternalNewsApiClient apiClient,
    IArticleSyncLogRepository repository,
    ILogger<SyncCheckedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleCheckedEvent message, CancellationToken cancellationToken = default)
    {
        if (await repository.IsSyncedAsync(message.CheckedArticleId, cancellationToken))
        {
            logger.LogInformation("Checked article {CheckedArticleId} was already synced", message.CheckedArticleId);
            return;
        }

        var syncLog = new ArticleSyncLog
        {
            CheckedArticleId = message.CheckedArticleId,
            ScrapedArticleId = message.ScrapedArticleId,
            Attempts = 1
        };

        try
        {
            syncLog.ExternalId = await apiClient.SendAsync(message, cancellationToken);
            syncLog.Status = "Synced";
            syncLog.SyncedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            syncLog.Status = "Failed";
            syncLog.ErrorMessage = ex.Message;
            await repository.AddAsync(syncLog, cancellationToken);
            throw;
        }

        await repository.AddAsync(syncLog, cancellationToken);
        logger.LogInformation("Synced checked article {CheckedArticleId}", message.CheckedArticleId);
    }
}

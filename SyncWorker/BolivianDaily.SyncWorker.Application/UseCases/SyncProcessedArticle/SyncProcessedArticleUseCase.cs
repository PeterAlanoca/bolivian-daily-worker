using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Domain.Entities;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.SyncWorker.Application.UseCases.SyncProcessedArticle;

public sealed class SyncProcessedArticleUseCase(
    IExternalNewsApiClient apiClient,
    IArticleSyncLogRepository repository,
    ILogger<SyncProcessedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleProcessedEvent message, CancellationToken cancellationToken = default)
    {
        if (await repository.IsSyncedAsync(message.ProcessedArticleId, cancellationToken))
        {
            logger.LogInformation("Processed article {ProcessedArticleId} was already synced", message.ProcessedArticleId);
            return;
        }

        var syncLog = new ArticleSyncLog
        {
            ProcessedArticleId = message.ProcessedArticleId,
            SourceArticleId = message.SourceArticleId,
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
        logger.LogInformation("Synced processed article {ProcessedArticleId}", message.ProcessedArticleId);
    }
}

using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Application.Mappers;
using BolivianDaily.SyncWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;

public sealed class SyncCheckedArticleUseCase(
    IArticleSyncer articleSyncer,
    ISyncedArticleRepository syncedArticleRepository,
    ILogger<SyncCheckedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleCheckedEvent articleCheckedEvent, CancellationToken cancellationToken = default)
    {
        if (await syncedArticleRepository.IsSyncedAsync(articleCheckedEvent.CheckedArticleId, cancellationToken))
        {
            logger.LogInformation("Checked article {CheckedArticleId} was already synced", articleCheckedEvent.CheckedArticleId);
            return;
        }

        var syncedArticle = articleCheckedEvent.ToSyncedArticle();

        try
        {
            var articleSyncResult = await articleSyncer.SyncAsync(articleCheckedEvent, cancellationToken);
            syncedArticle.ExtranetId = articleSyncResult.Id;
            syncedArticle.ExtranetUrl = articleSyncResult.Url;
            syncedArticle.Status = articleSyncResult.Status ?? "SYNCED";
            syncedArticle.Details = articleSyncResult.Message;
            syncedArticle.SyncedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            syncedArticle.Status = "FAILED";
            syncedArticle.Details = ex.Message;
            await syncedArticleRepository.AddAsync(syncedArticle, cancellationToken);
            throw;
        }

        await syncedArticleRepository.AddAsync(syncedArticle, cancellationToken);
        logger.LogInformation("Synced checked article {CheckedArticleId}", articleCheckedEvent.CheckedArticleId);
    }
}

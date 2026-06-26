using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Application.Mappers;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.CheckerWorker.Application.UseCases.ProcessScrapedArticle;

public sealed class ProcessScrapedArticleUseCase(
    IArticleChecker articleChecker,
    IProcessedArticleRepository processedArticleRepository,
    IArticleProcessedEventPublisher eventPublisher,
    ILogger<ProcessScrapedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default)
    {
        if (await processedArticleRepository.ExistsForScrapedArticleAsync(message.ArticleId, cancellationToken))
        {
            logger.LogInformation("Article {ArticleId} was already processed", message.ArticleId);
            return;
        }

        var processedArticle = await articleChecker.CheckAsync(message, cancellationToken);
        await processedArticleRepository.AddAsync(processedArticle, cancellationToken);

        if (!processedArticle.IsValid)
        {
            logger.LogWarning(
                "Article {ArticleId} is not valid — will not be published. Warnings: {Warnings}",
                message.ArticleId,
                processedArticle.Warnings);
            return;
        }

        await eventPublisher.PublishAsync(processedArticle.AsScrapedEvent(), cancellationToken);

        logger.LogInformation(
            "Processed and published article {ArticleId} as {ProcessedArticleId}",
            message.ArticleId,
            processedArticle.Id);
    }
}

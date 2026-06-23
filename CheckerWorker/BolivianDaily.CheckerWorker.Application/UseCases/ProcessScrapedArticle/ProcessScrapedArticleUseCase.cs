using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.CheckerWorker.Application.UseCases.ProcessScrapedArticle;

public sealed class ProcessScrapedArticleUseCase(
    IAiArticleChecker aiArticleChecker,
    IProcessedArticleRepository repository,
    IArticleProcessedEventPublisher eventPublisher,
    ILogger<ProcessScrapedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsForSourceArticleAsync(message.ArticleId, cancellationToken))
        {
            logger.LogInformation("Article {ArticleId} was already processed", message.ArticleId);
            return;
        }

        var processedArticle = await aiArticleChecker.CheckAsync(message, cancellationToken);
        await repository.AddAsync(processedArticle, cancellationToken);
        await eventPublisher.PublishAsync(processedArticle.ToEvent(), cancellationToken);

        logger.LogInformation(
            "Processed article {ArticleId} as {ProcessedArticleId}",
            message.ArticleId,
            processedArticle.Id);
    }
}

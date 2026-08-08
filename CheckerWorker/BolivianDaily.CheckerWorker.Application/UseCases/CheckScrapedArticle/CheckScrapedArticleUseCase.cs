using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Application.Mappers;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.CheckerWorker.Application.UseCases.CheckScrapedArticle;

public sealed class CheckScrapedArticleUseCase(
    IArticleChecker articleChecker,
    ICheckedArticleRepository checkedArticleRepository,
    IArticleCheckedEventPublisher eventPublisher,
    ILogger<CheckScrapedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleScrapedEvent message, CancellationToken cancellationToken = default)
    {
        if (await checkedArticleRepository.ExistsForScrapedArticleAsync(message.ArticleId, cancellationToken))
        {
            logger.LogInformation("Article {ArticleId} was already checked", message.ArticleId);
            return;
        }

        var checkedArticle = await articleChecker.CheckAsync(message, cancellationToken);
        await checkedArticleRepository.AddAsync(checkedArticle, cancellationToken);

        if (!checkedArticle.IsValid)
        {
            logger.LogWarning(
                "Article {ArticleId} is not valid — will not be published. Warnings: {Warnings}",
                message.ArticleId,
                checkedArticle.Warnings);
            return;
        }

        await eventPublisher.PublishAsync(checkedArticle.ToCheckedEvent(), cancellationToken);

        logger.LogInformation(
            "Checked and published article {ArticleId} as {CheckedArticleId}",
            message.ArticleId,
            checkedArticle.Id);
    }
}

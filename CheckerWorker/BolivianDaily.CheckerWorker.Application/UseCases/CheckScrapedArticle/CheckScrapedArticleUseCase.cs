using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Application.Mappers;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.CheckerWorker.Application.UseCases.CheckScrapedArticle;

public sealed class CheckScrapedArticleUseCase(
    IArticleChecker articleChecker,
    ICheckedArticleRepository checkedArticleRepository,
    IArticleCheckedEventPublisher articleCheckedEventPublisher,
    ILogger<CheckScrapedArticleUseCase> logger)
{
    public async Task ExecuteAsync(ArticleScrapedEvent articleScrapedEvent, CancellationToken cancellationToken = default)
    {
        if (await checkedArticleRepository.ExistsForScrapedArticleAsync(articleScrapedEvent.ArticleId, cancellationToken))
        {
            logger.LogInformation("Article {ArticleId} was already checked", articleScrapedEvent.ArticleId);
            return;
        }

        var checkedArticle = await articleChecker.CheckAsync(articleScrapedEvent, cancellationToken);
        await checkedArticleRepository.AddAsync(checkedArticle, cancellationToken);

        if (!checkedArticle.IsValid)
        {
            logger.LogWarning(
                "Article {ArticleId} is not valid — will not be published. Warnings: {Warnings}",
                articleScrapedEvent.ArticleId,
                checkedArticle.Warnings);
            return;
        }

        await articleCheckedEventPublisher.PublishAsync(checkedArticle.ToCheckedEvent(), cancellationToken);

        logger.LogInformation(
            "Checked and published article {ArticleId} as {CheckedArticleId}",
            articleScrapedEvent.ArticleId,
            checkedArticle.Id);
    }
}

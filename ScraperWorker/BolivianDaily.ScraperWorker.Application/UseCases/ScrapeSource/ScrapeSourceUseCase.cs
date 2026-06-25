using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;
using BolivianDaily.ScraperWorker.Application.Mappers;

namespace BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

public class ScrapeSourceUseCase(
    INewsSourceRepository sourceRepository,
    IArticleRepository articleRepository,
    INewsSourceParserRegistry parserRegistry,
    IArticleScrapedEventPublisher eventPublisher,
    ILogger<ScrapeSourceUseCase> logger)
{

    public async Task<ScrapeSourceResult> ExecuteAsync(ScrapeSourceCommand command, CancellationToken cancellationToken = default)
    {
        var source = await sourceRepository.GetActiveByAliasAsync(command.SourceAlias, cancellationToken);
        if (source is null)
        {
            logger.LogWarning("No active source found for alias {SourceAlias}", command.SourceAlias);
            return new ScrapeSourceResult(command.SourceAlias, 0, 0, 0, 0);
        }

        if (!parserRegistry.HasParserFor(source.Alias))
        {
            logger.LogWarning("No parser registered for source alias {SourceAlias}", source.Alias);
            return new ScrapeSourceResult(source.Alias, 0, 0, 0, 0);
        }

        var parser = parserRegistry.GetFor(source.Alias);
        var categoriesProcessed = 0;
        var urlsFound = 0;
        var scraped = 0;
        var skipped = 0;

        foreach (var sourceCategory in source.Categories)
        {
            if (categoriesProcessed > 0)
            {
                await Task.Delay(command.CategoryDelayMs, cancellationToken);
            }

            categoriesProcessed++;
            logger.LogInformation("Scraping {Source} category {Category}", source.Name, sourceCategory.Name);

            var urls = await parser.GetLatestArticleUrlsAsync(sourceCategory, cancellationToken);
            urlsFound += urls.Count;

            foreach (var url in urls)
            {
                if (await articleRepository.ExistsByUrlAsync(url, cancellationToken))
                {
                    skipped++;
                    logger.LogInformation("Skipping existing article {Url}", url);
                    continue;
                }

                await Task.Delay(new Random().Next(command.MinArticleDelayMs, command.MaxArticleDelayMs), cancellationToken);

                var article = await parser.ParseArticleAsync(url, cancellationToken);
                if (article is null)
                {
                    skipped++;
                    continue;
                }

                var category = sourceCategory.Category
                 ?? throw new InvalidOperationException($"Category not loaded for SourceCategory {sourceCategory.Id}");

                article.NewsSourceId = source.Id;
                article.CategoryId = category.Id;
                article.SourceCategoryId = sourceCategory.Id;

                await articleRepository.AddAsync(article, cancellationToken);
                scraped++;

                logger.LogInformation("Scraped article: {Title}", article.Title);

                var scrapedEvent = article.AsScrapedEvent(source, category);
                await eventPublisher.PublishAsync(scrapedEvent, cancellationToken);

                logger.LogInformation("Scraped queue article: {Title}", article.Title);
            }
        }

        return new ScrapeSourceResult(source.Alias, categoriesProcessed, urlsFound, scraped, skipped);
    }
}

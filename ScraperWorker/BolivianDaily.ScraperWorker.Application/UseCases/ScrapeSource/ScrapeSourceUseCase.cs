using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;
using BolivianDaily.ScraperWorker.Application.Mappers;

namespace BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

public class ScrapeSourceUseCase(
    INewsSourceRepository newsSourceRepository,
    IArticleRepository articleRepository,
    INewsSourceParserRegistry newsSourceParserRegistry,
    IArticleScrapedEventPublisher articleScrapedEventPublisher,
    ILogger<ScrapeSourceUseCase> logger)
{

    public async Task<ScrapeSourceResult> ExecuteAsync(ScrapeSourceCommand scrapeSourceCommand, CancellationToken cancellationToken = default)
    {
        var newsSource = await newsSourceRepository.GetActiveByAliasAsync(scrapeSourceCommand.SourceAlias, cancellationToken);
        if (newsSource is null)
        {
            logger.LogWarning("No active source found for alias {SourceAlias}", scrapeSourceCommand.SourceAlias);
            return new ScrapeSourceResult(scrapeSourceCommand.SourceAlias, 0, 0, 0, 0);
        }

        if (!newsSourceParserRegistry.HasParserFor(newsSource.Alias))
        {
            logger.LogWarning("No parser registered for source alias {SourceAlias}", newsSource.Alias);
            return new ScrapeSourceResult(newsSource.Alias, 0, 0, 0, 0);
        }

        var parser = newsSourceParserRegistry.GetFor(newsSource.Alias);
        var categoriesProcessed = 0;
        var urlsFound = 0;
        var scraped = 0;
        var skipped = 0;

        foreach (var sourceCategory in newsSource.Categories)
        {
            if (categoriesProcessed > 0)
            {
                await Task.Delay(scrapeSourceCommand.CategoryDelayMs, cancellationToken);
            }

            categoriesProcessed++;
            logger.LogInformation("Scraping {Source} category {Category}", newsSource.Name, sourceCategory.Name);

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

                await Task.Delay(new Random().Next(scrapeSourceCommand.MinArticleDelayMs, scrapeSourceCommand.MaxArticleDelayMs), cancellationToken);

                var article = await parser.ParseArticleAsync(url, cancellationToken);
                if (article is null)
                {
                    skipped++;
                    continue;
                }

                var category = sourceCategory.Category
                 ?? throw new InvalidOperationException($"Category not loaded for SourceCategory {sourceCategory.Id}");

                article.NewsSourceId = newsSource.Id;
                article.CategoryId = category.Id;
                article.SourceCategoryId = sourceCategory.Id;

                await articleRepository.AddAsync(article, cancellationToken);
                scraped++;

                logger.LogInformation("Scraped article: {Title}", article.Title);

                var articleScrapedEvent = article.AsScrapedEvent(newsSource, category);
                await articleScrapedEventPublisher.PublishAsync(articleScrapedEvent, cancellationToken);

                logger.LogInformation("Scraped queue article: {Title}", article.Title);
            }
        }

        return new ScrapeSourceResult(newsSource.Alias, categoriesProcessed, urlsFound, scraped, skipped);
    }
}

using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

public class ScrapeSourceUseCase
{
    private readonly INewsSourceRepository _sourceRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly INewsSourceParserRegistry _parserRegistry;
    private readonly ILogger<ScrapeSourceUseCase> _logger;

    public ScrapeSourceUseCase(
        INewsSourceRepository sourceRepository,
        IArticleRepository articleRepository,
        INewsSourceParserRegistry parserRegistry,
        ILogger<ScrapeSourceUseCase> logger)
    {
        _sourceRepository = sourceRepository;
        _articleRepository = articleRepository;
        _parserRegistry = parserRegistry;
        _logger = logger;
    }

    public async Task<ScrapeSourceResult> ExecuteAsync(ScrapeSourceCommand command, CancellationToken cancellationToken = default)
    {
        var source = await _sourceRepository.GetActiveByAliasAsync(command.SourceAlias, cancellationToken);
        if (source is null)
        {
            _logger.LogWarning("No active source found for alias {SourceAlias}", command.SourceAlias);
            return new ScrapeSourceResult(command.SourceAlias, 0, 0, 0, 0);
        }

        if (!_parserRegistry.HasParserFor(source.Alias))
        {
            _logger.LogWarning("No parser registered for source alias {SourceAlias}", source.Alias);
            return new ScrapeSourceResult(source.Alias, 0, 0, 0, 0);
        }

        var parser = _parserRegistry.GetFor(source.Alias);
        var categoriesProcessed = 0;
        var urlsFound = 0;
        var scraped = 0;
        var skipped = 0;

        foreach (var sourceCategory in source.Categories.Where(c => c.State == "A"))
        {
            categoriesProcessed++;
            _logger.LogInformation("Scraping {Source} category {Category}", source.Name, sourceCategory.Name);

            var urls = await parser.GetLatestArticleUrlsAsync(sourceCategory, cancellationToken);
            var selectedUrls = urls.Take(command.MaxArticlesPerCategory).ToList();
            urlsFound += selectedUrls.Count;

            foreach (var url in selectedUrls)
            {
                if (await _articleRepository.ExistsByUrlAsync(url, cancellationToken))
                {
                    skipped++;
                    _logger.LogInformation("Skipping existing article {Url}", url);
                    continue;
                }

                var article = await parser.ParseArticleAsync(url, cancellationToken);
                if (article is null)
                {
                    skipped++;
                    continue;
                }

                article.NewsSourceId = source.Id;
                article.CategoryId = sourceCategory.CategoryId;
                article.SourceCategoryId = sourceCategory.Id;

                await _articleRepository.AddAsync(article, cancellationToken);
                scraped++;

                _logger.LogInformation("Scraped article: {Title}", article.Title);
            }
        }

        return new ScrapeSourceResult(source.Alias, categoriesProcessed, urlsFound, scraped, skipped);
    }
}

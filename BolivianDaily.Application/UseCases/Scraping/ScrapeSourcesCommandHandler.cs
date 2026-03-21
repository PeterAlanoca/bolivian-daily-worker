namespace BolivianDaily.Application.UseCases.Scraping;

using MediatR;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class ScrapeSourcesCommandHandler : IRequestHandler<ScrapeSourcesCommand, bool>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly INewsRepository _newsRepository;
    private readonly IScraperFactory _scraperFactory;
    private readonly IExternalNewsApiClient _apiClient;
    private readonly ILogger<ScrapeSourcesCommandHandler> _logger;

    public ScrapeSourcesCommandHandler(
        ISourceRepository sourceRepository,
        INewsRepository newsRepository,
        IScraperFactory scraperFactory,
        IExternalNewsApiClient apiClient,
        ILogger<ScrapeSourcesCommandHandler> logger)
    {
        _sourceRepository = sourceRepository;
        _newsRepository = newsRepository;
        _scraperFactory = scraperFactory;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<bool> Handle(ScrapeSourcesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting scraping process...");
        var activeSources = await _sourceRepository.GetAllActiveAsync(cancellationToken);

        foreach (var sourceItem in activeSources)
        {
            if (!_scraperFactory.HasScraperFor(sourceItem.Alias ?? string.Empty))
            {
                _logger.LogWarning("No scraper registered for source '{Name}' (alias: '{Alias}'). Skipping.", sourceItem.Name, sourceItem.Alias);
                continue;
            }

            // Resolve the portal-specific scraper via Strategy Pattern
            var scraper = _scraperFactory.GetFor(sourceItem.Alias!);
            _logger.LogInformation("Processing source: {Name} using {Scraper}", sourceItem.Name, scraper.GetType().Name);

            foreach (var sourceCategory in sourceItem.Categories)
            {
                var articleUrls = await scraper.GetLatestArticleUrlsAsync(sourceCategory, cancellationToken);
                _logger.LogInformation("Found {Count} articles for category '{Category}'", articleUrls.Count, sourceCategory.Category?.Name);

                foreach (var url in articleUrls)
                {
                    var exists = await _newsRepository.ExistsByUrlAsync(url, cancellationToken);
                    if (exists) continue;

                    try
                    {
                        var newsArticle = await scraper.ScrapeArticleAsync(url, cancellationToken);
                        if (newsArticle != null)
                        {
                            newsArticle.CategoryId = sourceCategory.CategoryId;
                            newsArticle.SourceId = sourceItem.Id;

                           // await _newsRepository.AddAsync(newsArticle, cancellationToken);
                            //await _apiClient.SubmitNewsAsync(newsArticle, cancellationToken);

                            //_logger.LogInformation("Successfully processed article: {Title}", newsArticle.Title);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process article at {Url}", url);
                    }
                }
            }
        }

        _logger.LogInformation("Scraping process finished.");
        return true;
    }
}

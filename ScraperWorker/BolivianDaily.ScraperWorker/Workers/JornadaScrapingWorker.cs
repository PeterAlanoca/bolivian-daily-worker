using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;
using BolivianDaily.ScraperWorker.Infrastructure.Configuration;

namespace BolivianDaily.ScraperWorker.Workers;

public class JornadaScrapingWorker(
    IServiceProvider serviceProvider,
    INewsSourceOptionsProvider<JornadaOptions> jornadaOptionsProvider,
    ILogger<JornadaScrapingWorker> logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var interval = TimeSpan.FromMinutes(jornadaOptionsProvider.IntervalMinutes);

        logger.LogInformation("Jornada scraping worker started. Interval: {IntervalMinutes} minutes", jornadaOptionsProvider.IntervalMinutes);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                logger.LogDebug("Jornada starting synchronization...");

                using var scope = serviceProvider.CreateScope();
                var scrapeSourceUseCase = scope.ServiceProvider.GetRequiredService<ScrapeSourceUseCase>();

                var result = await scrapeSourceUseCase.ExecuteAsync(
                    new ScrapeSourceCommand(
                        jornadaOptionsProvider.Alias, 
                        jornadaOptionsProvider.CategoryDelayMs, 
                        jornadaOptionsProvider.MinArticleDelayMs,
                        jornadaOptionsProvider.MaxArticleDelayMs),
                    cancellationToken);

                logger.LogInformation(
                    "Jornada scraping completed. Categories: {Categories}, URLs: {Urls}, Scraped: {Scraped}, Skipped: {Skipped}",
                    result.CategoriesProcessed,
                    result.ArticleUrlsFound,
                    result.ArticlesScraped,
                    result.ArticlesSkipped);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Jornada Sync was canceled during execution.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while running Jornada scraping");
            }
            await Task.Delay(interval, cancellationToken);
        }

        logger.LogInformation("Jornada Worker is shutting down.");
    }
}

using BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

namespace BolivianDaily.ScraperWorker.Workers;

public class JornadaScrapingWorker(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<JornadaScrapingWorker> logger) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = configuration.GetValue("Scraping:Jornada:IntervalMinutes", 60);
        var interval = TimeSpan.FromMinutes(intervalMinutes);

        logger.LogInformation("Jornada scraping worker started. Interval: {IntervalMinutes} minutes", intervalMinutes);

        await RunOnceAsync(stoppingToken);

    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ScrapeSourceUseCase>();

            var result = await useCase.ExecuteAsync(
                new ScrapeSourceCommand("jornada"),
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while running Jornada scraping");
        }
    }
}

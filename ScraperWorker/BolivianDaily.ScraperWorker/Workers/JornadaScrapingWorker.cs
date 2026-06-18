using BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;

namespace BolivianDaily.ScraperWorker.Workers;

public class JornadaScrapingWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JornadaScrapingWorker> _logger;

    public JornadaScrapingWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<JornadaScrapingWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = _configuration.GetValue("Scraping:Jornada:IntervalMinutes", 60);
        var maxArticlesPerCategory = _configuration.GetValue("Scraping:Jornada:MaxArticlesPerCategory", 3);
        var runOnStartup = _configuration.GetValue("Scraping:Jornada:RunOnStartup", true);
        var interval = TimeSpan.FromMinutes(intervalMinutes);

        _logger.LogInformation("Jornada scraping worker started. Interval: {IntervalMinutes} minutes", intervalMinutes);

        if (runOnStartup)
        {
            await RunOnceAsync(maxArticlesPerCategory, stoppingToken);
        }

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnceAsync(maxArticlesPerCategory, stoppingToken);
        }
    }

    private async Task RunOnceAsync(int maxArticlesPerCategory, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<ScrapeSourceUseCase>();

            var result = await useCase.ExecuteAsync(
                new ScrapeSourceCommand("jornada", maxArticlesPerCategory),
                cancellationToken);

            _logger.LogInformation(
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
            _logger.LogError(ex, "Unexpected error while running Jornada scraping");
        }
    }
}

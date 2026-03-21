namespace BolivianDaily.Worker;

using MediatR;
using BolivianDaily.Application.UseCases.Scraping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

public class ScraperWorker : BackgroundService
{
    private readonly ILogger<ScraperWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _period = TimeSpan.FromMinutes(15); 

    public ScraperWorker(ILogger<ScraperWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scraping Worker started.");

        using var timer = new PeriodicTimer(_period);
        
        do
        {
            try
            {
                _logger.LogInformation("Job executed at: {time}", DateTimeOffset.Now);

                // We must create a specific DI scope because DbContext and Repositories are typically scoped.
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(new ScrapeSourcesCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A fatal error occurred outside of the command handler.");
            }

        } while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }
}

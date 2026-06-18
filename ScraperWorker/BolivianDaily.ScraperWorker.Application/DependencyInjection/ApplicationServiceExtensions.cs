using BolivianDaily.ScraperWorker.Application.UseCases.ScrapeSource;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.ScraperWorker.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddScraperApplication(this IServiceCollection services)
    {
        services.AddTransient<ScrapeSourceUseCase>();
        return services;
    }
}

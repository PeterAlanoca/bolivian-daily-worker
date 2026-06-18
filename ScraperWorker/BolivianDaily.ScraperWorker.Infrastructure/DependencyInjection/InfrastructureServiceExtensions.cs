using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using BolivianDaily.ScraperWorker.Infrastructure.Parsers;
using BolivianDaily.ScraperWorker.Infrastructure.Persistence;
using BolivianDaily.ScraperWorker.Infrastructure.Scraping;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.ScraperWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddScraperInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IArticleRepository, InMemoryArticleRepository>();
        services.AddSingleton<INewsSourceRepository, InMemoryNewsSourceRepository>();

        services.AddHttpClient<HtmlDocumentFetcher>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("BolivianDailyScraperWorker/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddTransient<INewsSourceParser, JornadaParser>();
        services.AddTransient<INewsSourceParserRegistry, NewsSourceParserRegistry>();

        return services;
    }
}

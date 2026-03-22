namespace BolivianDaily.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BolivianDaily.Domain.Repositories;
using BolivianDaily.Infrastructure.Data;
using BolivianDaily.Infrastructure.Repositories;
using BolivianDaily.Application.Interfaces;
using BolivianDaily.Infrastructure.Services;
using BolivianDaily.Infrastructure.Services.Scrapers;
using BolivianDaily.Infrastructure.ExternalApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
                   .UseSnakeCaseNamingConvention()); 

        // Repositories
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<ISourceRepository, SourceRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Register each portal-specific scraper with its own named HttpClient
        services.AddHttpClient<JornadaScraperService>();
        services.AddHttpClient<ElDeberScraperService>();

        // Register the ScraperFactory as a singleton: it holds references to all scrapers
        services.AddSingleton<ScraperFactory>();
        services.AddSingleton<IScraperFactory>(sp => sp.GetRequiredService<ScraperFactory>());

        // External API client
        services.AddHttpClient<IExternalNewsApiClient, ExternalNewsApiClient>();

        return services;
    }
}


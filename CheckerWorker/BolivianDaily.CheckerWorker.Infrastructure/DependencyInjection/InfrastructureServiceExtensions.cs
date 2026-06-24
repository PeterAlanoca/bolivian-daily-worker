using System.Net.Http.Headers;
using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.CheckerWorker.Infrastructure.Configuration;
using BolivianDaily.CheckerWorker.Infrastructure.Messaging;
using BolivianDaily.CheckerWorker.Infrastructure.OpenRouter;
using BolivianDaily.CheckerWorker.Infrastructure.Persistence;
using BolivianDaily.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BolivianDaily.CheckerWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddCheckerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<OpenRouterOptions>(configuration.GetSection(OpenRouterOptions.SectionName));

        services.AddDbContext<CheckerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProcessedArticleRepository, SqlProcessedArticleRepository>();
        services.AddScoped<IArticleProcessedEventPublisher, RabbitMqArticleProcessedEventPublisher>();

        services.AddHttpClient<IArticleChecker, OpenRouterArticleChecker>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OpenRouterOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        });

        return services;
    }
}

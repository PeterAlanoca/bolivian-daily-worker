using BolivianDaily.CheckerWorker.Application.Interfaces;
using BolivianDaily.CheckerWorker.Domain.Repositories;
using BolivianDaily.CheckerWorker.Infrastructure.AI;
using BolivianDaily.CheckerWorker.Infrastructure.Configuration;
using BolivianDaily.CheckerWorker.Infrastructure.Messaging;
using BolivianDaily.CheckerWorker.Infrastructure.Persistence;
using BolivianDaily.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.CheckerWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddCheckerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<OpenRouterOptions>(configuration.GetSection(OpenRouterOptions.SectionName));

        services.AddDbContext<CheckerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IProcessedArticleRepository, ProcessedArticleRepository>();
        services.AddScoped<IArticleProcessedEventPublisher, RabbitMqArticleProcessedEventPublisher>();
        services.AddHttpClient<IAiArticleChecker, OpenRouterArticleChecker>();

        return services;
    }
}

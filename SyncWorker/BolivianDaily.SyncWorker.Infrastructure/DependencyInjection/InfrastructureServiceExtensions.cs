using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Domain.Repositories;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using BolivianDaily.SyncWorker.Infrastructure.ExternalApi;
using BolivianDaily.SyncWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BolivianDaily.SyncWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<ExternalApiOptions>(configuration.GetSection(ExternalApiOptions.SectionName));

        services.AddDbContext<SyncDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SyncDb")));

        services.AddScoped<IArticleSyncLogRepository, ArticleSyncLogRepository>();
        services.AddHttpClient<IExternalNewsApiClient, BolivianDailyApiClient>();

        return services;
    }
}

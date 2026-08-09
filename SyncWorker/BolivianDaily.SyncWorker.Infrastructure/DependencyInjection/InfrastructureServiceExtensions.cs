using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Domain.Repositories;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using BolivianDaily.SyncWorker.Infrastructure.ExternalApi;
using BolivianDaily.SyncWorker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BolivianDaily.SyncWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddSyncInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<ExternalApiOptions>(configuration.GetSection(ExternalApiOptions.SectionName));

        services.AddDbContext<SyncDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(new UpdateTimestampInterceptor()));

        services.AddScoped<ISyncedArticleRepository, SyncedArticleRepository>();
        services.AddHttpClient<IExternalNewsApiClient, BolivianDailyApiClient>();

        services.AddSingleton<ConnectionFactory>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new ConnectionFactory
            {
                HostName = opts.HostName,
                Port = opts.Port,
                UserName = opts.UserName,
                Password = opts.Password,
                DispatchConsumersAsync = true
            };
        });

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = sp.GetRequiredService<ConnectionFactory>();
            return factory.CreateConnection();
        });

        return services;
    }
}

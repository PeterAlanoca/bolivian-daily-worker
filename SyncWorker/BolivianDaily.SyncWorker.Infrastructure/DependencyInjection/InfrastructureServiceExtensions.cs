using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.Interfaces;
using BolivianDaily.SyncWorker.Domain.Repositories;
using BolivianDaily.SyncWorker.Infrastructure.Configuration;
using BolivianDaily.SyncWorker.Infrastructure.Extranet;
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
        services.Configure<ExtranetOptions>(configuration.GetSection(ExtranetOptions.SectionName));

        services.AddDbContext<SyncDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(new UpdateTimestampInterceptor()));

        services.AddScoped<ISyncedArticleRepository, SyncedArticleRepository>();
        services.AddHttpClient("extranet", (sp, client) =>
        {
            var extranetOptions = sp.GetRequiredService<IOptions<ExtranetOptions>>().Value;
            client.BaseAddress = new Uri(extranetOptions.Url);
        });
        services.AddSingleton<ExtranetTokenProvider>(sp =>
            new ExtranetTokenProvider(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("extranet"),
                sp.GetRequiredService<IOptions<ExtranetOptions>>()));
        services.AddHttpClient<IArticleSyncer, ExtranetArticleSyncer>((sp, client) =>
        {
            var extranetOptions = sp.GetRequiredService<IOptions<ExtranetOptions>>().Value;
            client.BaseAddress = new Uri(extranetOptions.Url);
        });

        services.AddSingleton<ConnectionFactory>(sp =>
        {
            var rabbitMqOptions = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new ConnectionFactory
            {
                HostName = rabbitMqOptions.HostName,
                Port = rabbitMqOptions.Port,
                UserName = rabbitMqOptions.UserName,
                Password = rabbitMqOptions.Password,
                DispatchConsumersAsync = true
            };
        });

        services.AddSingleton<IConnection>(sp =>
        {
            var connectionFactory = sp.GetRequiredService<ConnectionFactory>();
            return connectionFactory.CreateConnection();
        });

        return services;
    }
}

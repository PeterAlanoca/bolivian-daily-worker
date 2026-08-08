using BolivianDaily.ScraperWorker.Application.Interfaces;
using BolivianDaily.ScraperWorker.Domain.Repositories;
using BolivianDaily.ScraperWorker.Infrastructure.Configuration;
using BolivianDaily.ScraperWorker.Infrastructure.Parsers;
using BolivianDaily.ScraperWorker.Infrastructure.Persistence;
using BolivianDaily.ScraperWorker.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BolivianDaily.ScraperWorker.Infrastructure.Messaging;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using BolivianDaily.Shared.Messaging;

namespace BolivianDaily.ScraperWorker.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddScraperInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ScraperDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(new UpdateTimestampInterceptor()));

        services.Configure<JornadaOptions>(configuration.GetSection(JornadaOptions.SectionName));
        services.Configure<ElDiarioOptions>(configuration.GetSection(ElDiarioOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton(typeof(INewsSourceOptionsProvider<>), typeof(NewsSourceOptionsProvider<>));

        services.AddScoped<IArticleRepository, SqlArticleRepository>();
        services.AddScoped<INewsSourceRepository, SqlNewsSourceRepository>();

        services.AddHttpClient<HtmlDocumentFetcher>(client =>
        {
            //client.DefaultRequestHeaders.UserAgent.ParseAdd("BolivianDailyScraperWorker/1.0");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddTransient<INewsSourceParser, JornadaParser>();
        services.AddTransient<INewsSourceParserRegistry, NewsSourceParserRegistry>();

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

        services.AddScoped<IArticleScrapedEventPublisher, RabbitMqArticleScrapedEventPublisher>();

        return services;
    }
}

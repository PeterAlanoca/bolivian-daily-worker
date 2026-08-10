using BolivianDaily.CheckerWorker.Application.UseCases.CheckScrapedArticle;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;

namespace BolivianDaily.CheckerWorker;

public sealed class CheckerConsumer(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<CheckerConsumer> logger) : RabbitMqConsumerHostedService<ArticleScrapedEvent>(serviceProvider, options, logger)
{
    protected override string QueueName => Options.Value.ArticleScrapedQueue;

    protected override string RoutingKey => Options.Value.ArticleScrapedRoutingKey;

    protected override async Task HandleAsync(ArticleScrapedEvent articleScrapedEvent, CancellationToken stoppingToken)
    {
        Logger.LogInformation("Checking scraped article {ArticleId} ({Title}) from {SourceName}", articleScrapedEvent.ArticleId, articleScrapedEvent.Title, articleScrapedEvent.SourceName);

        using var scope = ServiceProvider.CreateScope();
        var checkScrapedArticleUseCase = scope.ServiceProvider.GetRequiredService<CheckScrapedArticleUseCase>();
        await checkScrapedArticleUseCase.ExecuteAsync(articleScrapedEvent, stoppingToken);

        Logger.LogInformation("Article {ArticleId} checked and acked", articleScrapedEvent.ArticleId);
    }
}

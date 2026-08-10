using BolivianDaily.Shared.Messaging;
using BolivianDaily.SyncWorker.Application.UseCases.SyncCheckedArticle;
using Microsoft.Extensions.Options;

namespace BolivianDaily.SyncWorker;

public sealed class SyncConsumer(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<SyncConsumer> logger) : RabbitMqConsumerHostedService<ArticleCheckedEvent>(serviceProvider, options, logger)
{
    protected override string QueueName => Options.Value.ArticleCheckedQueue;

    protected override string RoutingKey => Options.Value.ArticleCheckedRoutingKey;

    protected override async Task HandleAsync(ArticleCheckedEvent articleCheckedEvent, CancellationToken stoppingToken)
    {
        Logger.LogInformation("Syncing checked article {ScrapedArticleId} -> {CheckedArticleId} ({Title})", articleCheckedEvent.ScrapedArticleId, articleCheckedEvent.CheckedArticleId, articleCheckedEvent.Title);

        using var scope = ServiceProvider.CreateScope();
        var syncCheckedArticleUseCase = scope.ServiceProvider.GetRequiredService<SyncCheckedArticleUseCase>();
        await syncCheckedArticleUseCase.ExecuteAsync(articleCheckedEvent, stoppingToken);

        Logger.LogInformation("Article {CheckedArticleId} synced and acked", articleCheckedEvent.CheckedArticleId);
    }
}

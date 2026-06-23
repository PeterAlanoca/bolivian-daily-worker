using System.Text;
using System.Text.Json;
using BolivianDaily.CheckerWorker.Application.UseCases.ProcessScrapedArticle;
using BolivianDaily.Shared.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BolivianDaily.CheckerWorker;

public sealed class Worker(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger<Worker> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private IConnection? connection;
    private IModel? channel;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var rabbitMqOptions = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqOptions.HostName,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.UserName,
            Password = rabbitMqOptions.Password,
            DispatchConsumersAsync = true
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(rabbitMqOptions.ArticleScrapedQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(
            rabbitMqOptions.ArticleScrapedQueue,
            rabbitMqOptions.Exchange,
            RabbitMqTopology.ArticleScrapedRoutingKey(rabbitMqOptions));
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<ArticleScrapedEvent>(json, JsonOptions);
                if (message is null)
                {
                    logger.LogWarning("Received invalid ArticleScrapedEvent payload");
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }

                using var scope = serviceProvider.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<ProcessScrapedArticleUseCase>();
                await useCase.ExecuteAsync(message, stoppingToken);
                channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing ArticleScrapedEvent");
                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsume(rabbitMqOptions.ArticleScrapedQueue, autoAck: false, consumer);
        logger.LogInformation("CheckerWorker consuming queue {Queue}", rabbitMqOptions.ArticleScrapedQueue);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        channel?.Dispose();
        connection?.Dispose();
        base.Dispose();
    }
}

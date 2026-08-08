using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BolivianDaily.Shared.Messaging;

public abstract class RabbitMqConsumerHostedService<TMessage>(
    IServiceProvider serviceProvider,
    IOptions<RabbitMqOptions> options,
    ILogger logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan MaxReconnectDelay = TimeSpan.FromSeconds(30);

    protected IServiceProvider ServiceProvider { get; } = serviceProvider;
    protected IOptions<RabbitMqOptions> Options { get; } = options;
    protected ILogger Logger { get; } = logger;

    protected abstract string QueueName { get; }
    protected abstract string RoutingKey { get; }
    protected abstract Task HandleAsync(TMessage message, CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IModel? channel = null;
        var delay = TimeSpan.FromSeconds(1);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var connection = ServiceProvider.GetRequiredService<IConnection>();
                channel = CreateConsumingChannel(connection, stoppingToken);
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogWarning(ex, "Unable to connect to RabbitMQ, retrying in {Delay}", delay);
                await Task.Delay(delay, stoppingToken);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, MaxReconnectDelay.TotalSeconds));
            }
        }

        if (channel is null)
        {
            return;
        }

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Stopping consumer for queue {Queue}", QueueName);
        }
        finally
        {
            channel.Dispose();
        }
    }

    private IModel CreateConsumingChannel(IConnection connection, CancellationToken stoppingToken)
    {
        var rabbitMqOptions = Options.Value;

        var channel = connection.CreateModel();
        RabbitMqTopology.Declare(channel, rabbitMqOptions, QueueName, RoutingKey);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var message = JsonSerializer.Deserialize<TMessage>(json, JsonOptions);
                if (message is null)
                {
                    Logger.LogWarning("Received invalid {MessageType} payload", typeof(TMessage).Name);
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }

                await HandleAsync(message, stoppingToken);
                channel.BasicAck(args.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing {MessageType}", typeof(TMessage).Name);
                HandleProcessingFailure(channel, args.DeliveryTag, args.BasicProperties, args.Body);
            }
        };

        channel.BasicConsume(QueueName, autoAck: false, consumer);
        Logger.LogInformation("Consuming queue {Queue} with routing key {RoutingKey}", QueueName, RoutingKey);

        return channel;
    }

    private void HandleProcessingFailure(IModel channel, ulong deliveryTag, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        var rabbitMqOptions = Options.Value;
        var retryCount = GetRetryCount(properties);

        if (retryCount < rabbitMqOptions.MaxRetries)
        {
            var retryProperties = channel.CreateBasicProperties();
            retryProperties.Persistent = properties.Persistent;
            retryProperties.ContentType = properties.ContentType;
            retryProperties.Type = properties.Type;
            retryProperties.Headers = new Dictionary<string, object>
            {
                ["x-retry-count"] = retryCount + 1
            };

            channel.BasicPublish(
                rabbitMqOptions.Exchange,
                RoutingKey,
                mandatory: false,
                basicProperties: retryProperties,
                body: body.ToArray());
            channel.BasicAck(deliveryTag, multiple: false);
            Logger.LogWarning("Requeued message {DeliveryTag} with retry count {RetryCount}", deliveryTag, retryCount + 1);
        }
        else
        {
            channel.BasicNack(deliveryTag, multiple: false, requeue: false);
            Logger.LogError("Message {DeliveryTag} exceeded max retries ({MaxRetries}), sent to dead letter queue", deliveryTag, rabbitMqOptions.MaxRetries);
        }
    }

    private static int GetRetryCount(IBasicProperties properties)
    {
        if (properties.Headers is null ||
            !properties.Headers.TryGetValue("x-retry-count", out var value))
        {
            return 0;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        if (value is byte[] bytes && bytes.Length == 4)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }

        return 0;
    }
}

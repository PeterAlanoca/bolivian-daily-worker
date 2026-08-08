using RabbitMQ.Client;

namespace BolivianDaily.Shared.Messaging;

public static class RabbitMqTopology
{
    public static string ArticleScrapedRoutingKey(RabbitMqOptions options) => options.ArticleScrapedRoutingKey;

    public static string ArticleCheckedRoutingKey(RabbitMqOptions options) => options.ArticleCheckedRoutingKey;

    public static string DeadLetterExchange(RabbitMqOptions options) => $"{options.Exchange}.dlx";

    public static string DeadLetterQueue(string queueName) => $"{queueName}.dead";

    public static void Declare(IModel channel, RabbitMqOptions options, string queueName, string routingKey)
    {
        var deadLetterExchange = DeadLetterExchange(options);
        var deadLetterQueue = DeadLetterQueue(queueName);

        channel.ExchangeDeclare(options.Exchange, ExchangeType.Direct, durable: true);
        channel.ExchangeDeclare(deadLetterExchange, ExchangeType.Direct, durable: true);
        channel.QueueDeclare(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = deadLetterExchange,
                ["x-dead-letter-routing-key"] = deadLetterQueue
            });
        channel.QueueDeclare(deadLetterQueue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queueName, options.Exchange, routingKey);
        channel.QueueBind(deadLetterQueue, deadLetterExchange, deadLetterQueue);
    }
}

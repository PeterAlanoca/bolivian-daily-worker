using RabbitMQ.Client;

namespace BolivianDaily.Shared.Messaging;

public static class RabbitMqTopology
{
    public static string ArticleScrapedRoutingKey(RabbitMqOptions rabbitMqOptions) => rabbitMqOptions.ArticleScrapedRoutingKey;

    public static string ArticleCheckedRoutingKey(RabbitMqOptions rabbitMqOptions) => rabbitMqOptions.ArticleCheckedRoutingKey;

    public static string DeadLetterExchange(RabbitMqOptions rabbitMqOptions) => $"{rabbitMqOptions.Exchange}.dlx";

    public static string DeadLetterQueue(string queueName) => $"{queueName}.dead";

    public static void Declare(IModel channel, RabbitMqOptions rabbitMqOptions, string queueName, string routingKey)
    {
        var deadLetterExchange = DeadLetterExchange(rabbitMqOptions);
        var deadLetterQueue = DeadLetterQueue(queueName);

        channel.ExchangeDeclare(rabbitMqOptions.Exchange, ExchangeType.Direct, durable: true);
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
        channel.QueueBind(queueName, rabbitMqOptions.Exchange, routingKey);
        channel.QueueBind(deadLetterQueue, deadLetterExchange, deadLetterQueue);
    }
}

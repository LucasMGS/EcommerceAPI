using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSE.Core.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NSE.Core.RabbitMQ;

public class RabbitMQConsumer : IQueueConsumer
{
    private readonly RabbitMQConnection _rabbitConnection;
    private readonly RabbitMQSettings _rabbitSettings;
    private readonly ILogger<RabbitMQConsumer> _logger;

    public RabbitMQConsumer(
        RabbitMQConnection rabbitConnection,
        ILogger<RabbitMQConsumer> logger,
        IOptions<RabbitMQSettings> rabbitSettings)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
        _rabbitSettings = rabbitSettings.Value;
    }

    public void StartConsuming<TMessage>(
        string queueName,
        ushort prefetchCount,
        Func<TMessage, Task<bool>> messageHandler,
        ushort? maxRetry = null)
    {
        var channel = _rabbitConnection.Connection.CreateModel();

        channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.Received += async (sender, eventArgs) =>
        {
            _logger.LogInformation("Message of Type {MessageType} received on {ConsumerType}",
                typeof(TMessage).Name,
                GetType().Name);

            try
            {
                var success = await messageHandler(eventArgs.GetDeserializedMessage<TMessage>());
                if (success)
                {
                    _logger.LogInformation("Finished processing the message of type {MessageType}", typeof(TMessage).Name);

                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

                    return;
                }

                NackWithRetryIncrement<TMessage>(eventArgs,channel,maxRetry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message of type {MessageType}", typeof(TMessage).Name);

                NackWithRetryIncrement<TMessage>(eventArgs, channel, maxRetry);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    private void NackWithRetryIncrement<TMessage>(
        BasicDeliverEventArgs args,
        IModel channel,
        ushort? maxRetry)
    {
        args.BasicProperties.IncrementRetryCountHeader();

        var maxRetryCount = maxRetry ?? _rabbitSettings.RetrySettings.Count;

        var retryCount = args.BasicProperties.GetRetryCount();
        var shouldRetry = retryCount <= maxRetryCount;

        _logger.LogInformation("Nacking message of type {MessageType} for the {RetryCount} time...", typeof(TMessage), retryCount);

        if (shouldRetry)
        {
            _logger.LogInformation("Requeuing message of type {MessateType} for retry...", typeof(TMessage));

            channel.BasicPublish(args.Exchange, args.RoutingKey, args.BasicProperties, args.Body);
        }

        channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
    }
}


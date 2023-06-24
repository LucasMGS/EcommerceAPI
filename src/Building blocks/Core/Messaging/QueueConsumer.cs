using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NSE.Core.Messaging
{
    public abstract class QueueConsumer<TMessage> : BackgroundService
    {
        private readonly ILogger<QueueConsumer<TMessage>> _logger;
        private readonly IQueueConsumer _consumer;
        private readonly string _queueName;
        private readonly ushort _prefetchCount;
        private readonly ushort? _retryCount;

        public QueueConsumer(
            ILogger<QueueConsumer<TMessage>> logger,
            IQueueConsumer consumer,
            string queueName,
            ushort prefetchCount = 1,
            ushort? retryCount = null)
        {
            _logger = logger;
            _consumer = consumer;
            _queueName = queueName;
            _prefetchCount = prefetchCount;
            _retryCount = retryCount;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.StartConsuming<TMessage>(_queueName, _prefetchCount, HandleMessage, _retryCount);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogDebug("Finish consuming [{QueueName}]", _queueName);
        }

        protected abstract Task<bool> HandleMessage(TMessage message);
    }
}


using Microsoft.Extensions.Logging;
using NSE.Core.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace NSE.Core.RabbitMQ
{
    public class RabbitMQProducer : IMessageProducer
    {
        private readonly RabbitMQConnection _rabbitConnection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQProducer> _logger;
        private IConnection Connection => _rabbitConnection.Connection;

        public RabbitMQProducer(
            RabbitMQConnection rabbitConnection,
            ILogger<RabbitMQProducer> logger)
        {
            _rabbitConnection = rabbitConnection;
            _channel = Connection.CreateModel();
            _logger = logger;
        }

        public bool Publish<TMessage>(TMessage message,string routingKey)
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            return PublishMessage(serializedMessage, routingKey);
        }

        private bool PublishMessage(string serializedMessage, string routingKey)
        {
            try
            {
                if (Connection is null) return false;

                var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);

                var props = _channel.CreateBasicProperties();
                props.Persistent = true;
                props.CreateRetryCountHeader();

                _channel.BasicPublish("", routingKey, props, messageBytes);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception captured on {Method} - {@DataPublished}", 
                    nameof(PublishMessage), 
                    new { serializedMessage, routingKey });
                return false;
            }
        }
    }
}

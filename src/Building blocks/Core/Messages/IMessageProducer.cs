namespace NSE.Core.Messages;

    public interface IMessageProducer
    {
        bool Publish<TMessage>(TMessage message, string routingKey);
    }


using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace NSE.Core.RabbitMQ;

public static class RabbitMQExtensions
{
    private const string RetryHeaderName = "x-retry-count";
    public static T GetDeserializedMessage<T>(this BasicDeliverEventArgs eventArgs)
    {
        var messageBody = Encoding.UTF8.GetString(eventArgs.Body.Span);
        return JsonSerializer.Deserialize<T>(messageBody);
    }

    public static int? GetRetryCount(this IBasicProperties props)
    {
        if (props.Headers is null) return null;

        return props.Headers.TryGetValue(RetryHeaderName, out var retryCountObj) ? (int)retryCountObj : null;
    }

    public static IBasicProperties CreateRetryCountHeader(this IBasicProperties props)
    {
        props.Headers = new Dictionary<string, object>
        {
            { RetryHeaderName, 0 }
        };

        return props;
    }

    public static IBasicProperties IncrementRetryCountHeader(this IBasicProperties props)
    {
        var retryCount = props.GetRetryCount();

        props.Headers[RetryHeaderName] = ++retryCount;

        return props;
    }
}


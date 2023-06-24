using NSE.Core.Messaging;
using NSE.Core.RabbitMQ;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace NSE.Customers.API.Bus;

public class QueueCreator
{
    private readonly IServiceProvider _serviceProvider;

    public QueueCreator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task CreateQueues()
    {
        var environment = _serviceProvider.GetRequiredService<IHostingEnvironment>();

        if (environment.EnvironmentName is not ("Internal" or "Development"))
        {
            return;
        }

        if (environment.IsEnvironment("Internal"))
        {
            await Task.Delay(10 * 1000);
        }

        using var scope = _serviceProvider.CreateScope();

        var rabbitConnection = _serviceProvider.GetRequiredService<RabbitMQConnection>();

        using var channel = rabbitConnection.Connection.CreateModel();

        foreach (var routingKey in QueueNames.All)
        {
            channel.QueueDeclare(queue: routingKey,
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }
    }
}

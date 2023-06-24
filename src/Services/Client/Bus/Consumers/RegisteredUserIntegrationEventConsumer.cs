using NSE.Core.Mediator;
using NSE.Core.Messages.Integration;
using NSE.Core.Messaging;
using NSE.Customers.API.Application.Commands;

namespace NSE.Customers.API.Bus.Consumers
{
    public class RegisteredUserIntegrationEventConsumer : QueueConsumer<RegisteredUserIntegrationEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        public RegisteredUserIntegrationEventConsumer(
            ILogger<QueueConsumer<RegisteredUserIntegrationEvent>> logger,
            IQueueConsumer consumer,
            IServiceProvider serviceProvider,
            ushort prefetchCount = 1,
            ushort? retryCount = null) : base(logger, consumer, QueueNames.RegisterCustomer, prefetchCount, retryCount)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<bool> HandleMessage(RegisteredUserIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();

            var mediatorHandler = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
            var command = new CreateCustomerCommand(message.Id, message.Nome, message.Email, message.Cpf);

            var result = await mediatorHandler.SendCommand(command);

            return true;
        }
    }
}

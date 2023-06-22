using MediatR;

namespace NSE.Customers.API.Application.Events
{
    public class CustomerEventHandler : INotificationHandler<CreatedCustomerEvent>
    {
        public Task Handle(CreatedCustomerEvent notification, CancellationToken cancellationToken)
        {
            // send event of confirmation (email,etc)
            return Task.CompletedTask;
        }
    }
}

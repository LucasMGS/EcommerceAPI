using FluentValidation.Results;
using MediatR;
using NSE.Client.API.Models;
using NSE.Core.Messages;
using NSE.Customers.API.Application.Events;
using NSE.Customers.API.Models;

namespace NSE.Customers.API.Application.Commands
{
    public class CreateCustomerCommandHandler : CommandHandler, IRequestHandler<CreateCustomerCommand, ValidationResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ValidationResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid()) return command.ValidationResult;

            var customer = new Customer(command.Id, command.Nome, command.Email, command.Cpf);

            var customerExists = await _customerRepository.GetByCPF(customer.Cpf.Numero);

            if(customerExists is not null)
            {
                AddError("Este CPF já está em uso!");
                return ValidationResult;
            }

            _customerRepository.Add(customer);

            customer.AddEvent(new CreatedCustomerEvent(command.Id, command.Nome, command.Email, command.Cpf));

            return await Commit(_customerRepository.UnitOfWork);
        }
    }
}

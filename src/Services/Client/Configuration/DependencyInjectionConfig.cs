using FluentValidation.Results;
using MediatR;
using NSE.Clientes.API.Data.Repository;
using NSE.Core.Mediator;
using NSE.Customers.API.Application.Commands;
using NSE.Customers.API.Application.Events;
using NSE.Customers.API.Data;
using NSE.Customers.API.Models;

namespace NSE.Clientes.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IRequestHandler<CreateCustomerCommand, ValidationResult>, CreateCustomerCommandHandler>();

            services.AddScoped<INotificationHandler<CreatedCustomerEvent>, CustomerEventHandler>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<CustomerContext>();
        }
    }
}
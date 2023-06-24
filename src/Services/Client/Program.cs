using NSE.Clientes.API.Configuration;
using NSE.Customers.API.Bus;
using NSE.Customers.API.Bus.Consumers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddApiConfiguration(builder.Configuration, builder.Environment);
builder.Services.RegisterServices();


builder.Services.AddHostedService<RegisteredUserIntegrationEventConsumer>();


builder.Services.AddMediatR(x =>
    x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

await new QueueCreator(app.Services).CreateQueues();
app.UseApiConfiguration(app.Environment);

app.Run();

using NSE.Core.Messaging;
using NSE.Core.RabbitMQ;
using NSE.Identity.API.Services;

namespace NSE.Identity.API.Configuration;

public static class Injector 
{
    public static IServiceCollection AddServices(this IServiceCollection services) {

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}

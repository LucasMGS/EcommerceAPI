using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSE.Core.Messaging;
using NSE.Core.RabbitMQ;
using NSE.Identidade.API.Extensions;
using NSE.WebAPI.Core.Identidade;
using RabbitMQ.Client;

namespace NSE.Identity.API.Configuration;

static class ApiConfiguration
{
    public static IServiceCollection AddApiConfiguration(this IServiceCollection service, ConfigurationManager configuration, IWebHostEnvironment environment){

        configuration.ConfigureJsonFiles(environment);

        service.AddDBConfig(configuration)
            .AddIdentity(configuration)
            .AddRabbitMQ(configuration);

        return service;
    }

    public static ConfigurationManager ConfigureJsonFiles(this ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        configuration.SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        if (environment.IsDevelopment())
        {
            configuration.AddUserSecrets<Program>();
        }

        return configuration;
    }

    public static IServiceCollection AddDBConfig(this IServiceCollection service, ConfigurationManager configuration){
    
        service.AddDbContext<ApplicationDBContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return service;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection service,ConfigurationManager configuration)
    {
        service.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddErrorDescriber<IdentityPortugueseMessages>()
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

        service.AddJwtConfiguration(configuration);

        return service;
    }

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, ConfigurationManager configuration)
    {
        var configurationSection = configuration.GetSection(nameof(RabbitMQSettings));
        var rabbitSettings = configurationSection.Get<RabbitMQSettings>();
        services.Configure<RabbitMQSettings>(configurationSection);

        services.AddSingleton<IConnectionFactory>(service => new ConnectionFactory
        {
            HostName = rabbitSettings.HostName,
            Port = rabbitSettings.Port,
            UserName = rabbitSettings.UserName,
            Password = rabbitSettings.Password,
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = 1,
            UseBackgroundThreadsForIO = false,
        });

        services.AddSingleton<RabbitMQConnection>();

        services.AddScoped<IMessageProducer, RabbitMQProducer>();

        return services;
    }
}
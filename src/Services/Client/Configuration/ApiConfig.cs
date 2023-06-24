using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSE.Core.Messaging;
using NSE.Core.RabbitMQ;
using NSE.Customers.API.Bus;
using NSE.Customers.API.Data;
using NSE.WebAPI.Core.Identidade;
using RabbitMQ.Client;

namespace NSE.Clientes.API.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment )
        {
            configuration.ConfigureJsonFiles(environment);

            services.AddDBContext(configuration)
                .AddCors()
                .AddRabbitMQ(configuration)
                .AddControllers();
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

        public static IServiceCollection AddDBContext(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<CustomerContext>(options =>
               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            return services;
        }

        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
               
                app.UseSwagger();
                app.UseSwaggerUI();
                
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }
            app.UseRouting();
            app.UseCors("Total");
            app.UseAuthConfiguration();
            app.MapControllers();
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, ConfigurationManager configuration)
        {
            var configurationSection = configuration.GetSection(nameof(RabbitMQSettings));
            var rabbitSettings = configurationSection.Get<RabbitMQSettings>();
            services.Configure<RabbitMQSettings>(configurationSection);

            services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(service => new ConnectionFactory
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
            services.AddSingleton<IQueueConsumer, RabbitMQConsumer>();

            return services;
        }
    }
}
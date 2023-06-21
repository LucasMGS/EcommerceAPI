using Microsoft.EntityFrameworkCore;
using NSE.Catalog.API.Data;

namespace NSE.Services.Catalog.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            configuration.SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                configuration.AddUserSecrets<Program>();
            }
            
            services.AddDbContext(configuration);
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("totalAccess", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CatalogContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void UseApiConfiguration(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("totalAccess");

            app.MapControllers();
        }
    }
}
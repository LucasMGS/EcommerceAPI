using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSE.Identidade.API.Extensions;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Identity.API.Configuration;

static class ApiConfiguration
{
  public static IServiceCollection AddApiConfiguration(this IServiceCollection service, ConfigurationManager configuration, IWebHostEnvironment environment){

    configuration.SetBasePath(environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

    if (environment.IsDevelopment()) {
      configuration.AddUserSecrets<Program>();
    }

    service
      .AddSwagger()
      .AddDBConfig(configuration)
      .AddIdentity(configuration);

    return service;
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

  public static IServiceCollection AddSwagger(this IServiceCollection service) {
   service.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NerdStore Enterprise Identity API",
        Description = "Esta API é sobre a loja nerdStore enterprise",
        Contact = new OpenApiContact { Name = "Lucas Moreira", Email = "lucasmgs21@gmail.com" },
        License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
    });
});
    return service;
  }
}

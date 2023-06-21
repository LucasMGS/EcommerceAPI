
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NSE.Identidade.API.Extensions;

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

    service.AddJWT(configuration)
      .AddSwagger()
      .AddDBConfig(configuration)
      .AddIdentity();

    return service;
  }

  public static IServiceCollection AddDBConfig(this IServiceCollection service, ConfigurationManager configuration){
    
    service.AddDbContext<ApplicationDBContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    return service;
  }

  public static IServiceCollection AddJWT(this IServiceCollection service, ConfigurationManager configuration)
  {
    var jwtSettings = configuration.GetSection("JWTSettings");
    service.Configure<JWTSettings>(jwtSettings);
    var jwtSettingsSection = jwtSettings.Get<JWTSettings>();
    var key = Encoding.ASCII.GetBytes(jwtSettingsSection.Secret);

    service.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(jwtOptions =>
    {
      jwtOptions.RequireHttpsMetadata = false;
      jwtOptions.SaveToken = true;
      jwtOptions.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettingsSection.Audience,
        ValidIssuer = jwtSettingsSection.Issuer,
      };
    });
    return service;
  }

  public static IServiceCollection AddIdentity(this IServiceCollection service)
  {
    service.AddDefaultIdentity<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddErrorDescriber<IdentityPortugueseMessages>()
        .AddEntityFrameworkStores<ApplicationDBContext>()
        .AddDefaultTokenProviders();

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
        License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org") }
    });
});
    return service;
  }
}

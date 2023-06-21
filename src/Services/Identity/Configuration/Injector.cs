using NSE.Identity.API.Services;

namespace NSE.Identity.API.Configuration;

public static class Injector {
  public static IServiceCollection AddServices(this IServiceCollection service) {
    service.AddScoped<IAuthService, AuthService>();
    return service;
  }
}

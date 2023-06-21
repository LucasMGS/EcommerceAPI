using Npgsql.Replication;
using NSE.Catalog.API.Data;
using NSE.Catalog.Repositories;


namespace NSE.Services.Catalog.Configuration
{
    public static class Injector
    {
        public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<CatalogContext>();
    }
}
}
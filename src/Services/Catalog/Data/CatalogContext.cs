using Microsoft.EntityFrameworkCore;
using NSE.Catalog.API.Models;

namespace NSE.Catalog.API.Data;

public class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

    public DbSet<Product> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var stringProperties = modelBuilder.Model
                                .GetEntityTypes()
                                .SelectMany(e => e.GetProperties()
                                .Where(p => p.ClrType == typeof(string)));
                                
        foreach (var property in stringProperties)
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }
}
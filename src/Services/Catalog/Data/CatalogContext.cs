using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using NSE.Catalog.API.Models;
using NSE.Core.Data;
using NSE.Core.Messages;

namespace NSE.Catalog.API.Data;

public class CatalogContext : DbContext, IUnitOfWork
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;   
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

        var stringProperties = modelBuilder.Model
                                .GetEntityTypes()
                                .SelectMany(e => e.GetProperties()
                                .Where(p => p.ClrType == typeof(string)));
                                
        foreach (var property in stringProperties)
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }
}
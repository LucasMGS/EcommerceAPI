using Microsoft.EntityFrameworkCore;
using NSE.Client.API.Models;
using NSE.Customers.API.Models;

namespace NSE.Customers.API.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Customer> Clients { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            var stringProperties = entityTypes
                                    .SelectMany(e => e.GetProperties()
                                    .Where(p => p.ClrType == typeof(string)));

            var foreignKeys = entityTypes.SelectMany(e => e.GetForeignKeys());
            foreach (var relationShip in foreignKeys)
                relationShip.DeleteBehavior = DeleteBehavior.ClientSetNull;

            foreach (var property in stringProperties)
                property.SetColumnType("varchar(100)");



            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;

            return sucesso;
        }
    }
}

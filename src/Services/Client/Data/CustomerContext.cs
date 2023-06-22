using Microsoft.EntityFrameworkCore;
using NSE.Client.API.Models;
using NSE.Core.Data;
using NSE.Core.DomainObjects;
using NSE.Core.Mediator;
using NSE.Customers.API.Models;

namespace NSE.Customers.API.Data
{
    public sealed class CustomerContext : DbContext, IUnitOfWork
    {
        private readonly IMediatorHandler _mediatorHandler;
        public CustomerContext(DbContextOptions<CustomerContext> options, IMediatorHandler mediatorHandler) 
                : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _mediatorHandler = mediatorHandler;
        }

        public DbSet<Customer> Customers { get; set; }
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
            if (sucesso) await _mediatorHandler.PublishEvents(this);
            return sucesso;
        }
    }

    public static class MediatorExtension
    {
        public static async Task PublishEvents<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                    .Entries<Entity>()
                    .Where(x => x.Entity.Notifications != null 
                            && x.Entity.Notifications.Any());

            var domainEvents = domainEntities
                    .SelectMany(x => x.Entity.Notifications!)
                    .ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.CleanEvents());

            var tasks = domainEvents.Select(async (domainEvent) =>
            {
                await mediator.PublishEvents(domainEvent);
            });

            await Task.WhenAll(tasks);

        }
    }
}

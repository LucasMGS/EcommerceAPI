using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using NSE.Catalog.API.Data;
using NSE.Catalog.API.Models;
using NSE.Core.Data;


namespace NSE.Catalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CatalogContext _context;

        public ProductRepository(CatalogContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void Add(Product product) => _context.Products.Add(product);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _context?.Dispose();
        }

        public async Task<IEnumerable<Product>> GetAll() => await _context.Products.AsNoTracking().ToListAsync();

        public async Task<Product?> GetById(Guid id) => await _context.Products.FindAsync(id);

        public void Update(Product product) => _context.Products.Update(product);
    }
}
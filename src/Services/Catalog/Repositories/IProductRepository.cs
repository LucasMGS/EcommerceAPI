using NSE.Catalog.API.Models;
using NSE.Core.Data;

namespace NSE.Catalog.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product?> GetById(Guid id);
        void Add(Product Product);
        void Update(Product product);
    }
}
using NSE.Client.API.Models;
using NSE.Core.Data;

namespace NSE.Customers.API.Models
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        void Add(Customer cliente);
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> GetByCPF(string cpf);
    }
}

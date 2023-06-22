using Microsoft.EntityFrameworkCore;
using NSE.Client.API.Models;
using NSE.Core.Data;
using NSE.Customers.API.Data;
using NSE.Customers.API.Models;

namespace NSE.Clientes.API.Data.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        public Task<Customer> GetByCPF(string cpf)
        {
            return _context.Customers.FirstOrDefaultAsync(c => c.Cpf.Numero == cpf);
        }

        public void Add(Customer cliente)
        {
            _context.Customers.Add(cliente);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
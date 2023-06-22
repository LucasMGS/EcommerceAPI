using NSE.Core.DomainObjects;
using NSE.Customers.API.Models;

namespace NSE.Client.API.Models
{
    public class Customer : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }
        public Email Email { get; private set; }
        public Cpf Cpf { get; private set; }
        public bool Excluido { get; private set; }
        public Address Endereco { get; private set; }

        // EF Relation
        protected Customer() { }

        public Customer(Guid id, string nome, string email, string cpf)
        {
            Id = id;
            Nome = nome;
            Email = new Email(email);
            Cpf = new Cpf(cpf);
            Excluido = false;
        }

        public void SetEmail(string email)
        {
            Email = new Email(email);
        }

        public void SetAddress(Address address)
        {
            Endereco = address;
        }
    }
}
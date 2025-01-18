using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public Guid CreateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        public void DeleteCustomer(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Customer> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        public Task<Customer> GetCustomerById(Guid id)
        {
            throw new NotImplementedException();
        }
        void ICustomerRepository.UpdateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}

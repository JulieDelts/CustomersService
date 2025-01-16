using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public List<Customer> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        void ICustomerRepository.CreateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        void ICustomerRepository.DeleteCustomer(int id)
        {
            throw new NotImplementedException();
        }
        Task<Customer> ICustomerRepository.GetCustomerById(int id)
        {
            throw new NotImplementedException();
        }

        void ICustomerRepository.UpdateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}

using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        void ICustomerRepository.CreateCustomer(Customer customer)
        {
            throw new NotImplementedException();
        }

        void ICustomerRepository.DeleteCustomer(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Customer> ICustomerRepository.GetAllCustomers()
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

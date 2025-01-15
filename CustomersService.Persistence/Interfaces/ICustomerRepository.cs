using CustomersService.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerById(int id);
        IEnumerable<Customer> GetAllCustomers();
        void CreateCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
    }
}

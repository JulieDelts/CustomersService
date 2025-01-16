using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerById(int id);
        List<Customer> GetAllCustomers();
        void CreateCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
    }
}

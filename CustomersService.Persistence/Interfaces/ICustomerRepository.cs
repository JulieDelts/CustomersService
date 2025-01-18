using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerById(Guid id);
        List<Customer> GetAllCustomers();
        Guid CreateCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Guid id);
    }
}

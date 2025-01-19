using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Guid> CreateAsync(Customer customer);
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task UpdateProfileAsync(Customer customer, Customer customerUpdate);
        Task UpdatePasswordAsync(Customer customer, string newPassword);
        Task ActivateAsync(Customer customer);
        Task DeactivateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}

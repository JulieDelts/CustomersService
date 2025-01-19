using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<Guid> CreateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProfileAsync(Customer customer, Customer customerUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePasswordAsync(Customer customer, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task ActivateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task DeactivateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}

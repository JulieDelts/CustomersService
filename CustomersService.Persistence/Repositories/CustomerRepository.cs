using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository(CustomerServiceDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
    {
        public async Task UpdateProfileAsync(Customer customer, Customer customerUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePasswordAsync(Customer customer, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override async Task ActivateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public override async Task DeactivateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}

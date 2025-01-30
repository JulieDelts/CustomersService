using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class CustomerRepository(CustomerServiceDbContext context) : BaseRepository<Customer>(context), ICustomerRepository
    {
        public async Task UpdateProfileAsync(Customer customer, Customer customerUpdate)
        {
            customer.FirstName = customerUpdate.FirstName;
            customer.LastName = customerUpdate.LastName;
            customer.Phone = customerUpdate.Phone;
            customer.Address = customerUpdate.Address;
            await context.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(Customer customer, string newPassword)
        {
            customer.Password = newPassword;
            await context.SaveChangesAsync();
        }

        public async Task ActivateAsync(Customer customer)
        {
            customer.IsDeactivated = false;
            await context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Customer customer)
        {
            customer.IsDeactivated = true;
            await context.SaveChangesAsync();
        }
    }
}

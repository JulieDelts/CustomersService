using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate)
        {
            customer.Role = Role.VIP;
            customer.CustomVipDueDate = vipExpirationDate;
            await context.SaveChangesAsync();
        }

        public async Task BatchUpdateRoleAsync(Dictionary<Customer, Role> customersWithRoles)
        {
            foreach (var customerWithRole in customersWithRoles)
            {
                var customer = customerWithRole.Key;
                var newRole = customerWithRole.Value;
                customer.Role = newRole;
            }

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

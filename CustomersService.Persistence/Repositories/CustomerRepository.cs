using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Persistence.Repositories;

public class CustomerRepository(
        CustomerServiceDbContext context,
        ILogger<CustomerRepository> logger) 
    : BaseRepository<Customer>(context), ICustomerRepository
{
    public async Task UpdateProfileAsync(Customer customer, Customer customerUpdate)
    {
        logger.LogDebug("Updating profile for customer {CustomerId}", customer.Id);
        logger.LogTrace("Customer update data: {@CustomerUpdate}", customerUpdate);

        customer.FirstName = customerUpdate.FirstName;
        customer.LastName = customerUpdate.LastName;
        customer.Phone = customerUpdate.Phone;
        customer.Address = customerUpdate.Address;

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully updated profile for customer {CustomerId}", customer.Id);
    }

    public async Task UpdatePasswordAsync(Customer customer, string newPassword)
    {
        logger.LogDebug("Updating password for customer {CustomerId}", customer.Id);

        customer.Password = newPassword;

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully updated password for customer {CustomerId}", customer.Id);
    }

    public async Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate)
    {
        logger.LogDebug("Setting VIP status for customer {CustomerId}", customer.Id);
        logger.LogTrace("VIP expiration date: {VipExpirationDate}", vipExpirationDate);

        customer.Role = Role.VIP;
        customer.CustomVipDueDate = vipExpirationDate;

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully set VIP status for customer {CustomerId}", customer.Id);
    }

    public async Task BatchUpdateRoleAsync(Dictionary<Customer, Role> customersWithRoles)
    {
        logger.LogDebug("Batch updating roles for customers");
        logger.LogTrace("Customers with roles data: {@CustomersWithRoles}", customersWithRoles);

        foreach (var customerWithRole in customersWithRoles)
        {
            var customer = customerWithRole.Key;
            var newRole = customerWithRole.Value;
            customer.Role = newRole;
        }

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully batch updated roles for customers");
    }

    public async Task ActivateAsync(Customer customer)
    {
        logger.LogDebug("Activating customer {CustomerId}", customer.Id);

        customer.IsDeactivated = false;

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully activated customer {CustomerId}", customer.Id);
    }

    public async Task DeactivateAsync(Customer customer)
    {
        logger.LogDebug("Deactivating customer {CustomerId}", customer.Id);

        customer.IsDeactivated = true;

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully deactivated customer {CustomerId}", customer.Id);
    }
}

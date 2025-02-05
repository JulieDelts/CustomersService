using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerRepository: IBaseRepository<Customer>, IDeactivatable<Customer>
    {
        Task UpdateProfileAsync(Customer customer, Customer customerUpdate);
        Task UpdatePasswordAsync(Customer customer, string newPassword);
        Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate);
        Task BatchUpdateRoleAsync(Dictionary<Customer, Role> customersWithRoles);
    }
}

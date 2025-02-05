
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces
{
    public interface ICustomerUnitOfWork
    {
        void Begin();
        void Commit();
        Task CreateCustomerAsync(Customer customer, Account account);
        void Rollback();
        Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate, List<Account> accounts);
        Task BatchUpdateRoleAsync(Dictionary<Customer, Role> customers, List<Account> accountsToActivate, List<Account> accountsToDeactivate);
    }
}

using CustomersService.Persistence.Entities;

namespace CustomersService.Persistence.Interfaces;

public interface ICustomerUnitOfWork
{
    void Begin();
    void Commit();
    Task CreateCustomerAsync(Customer customer, Account account);
    void Rollback();
    Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate, List<Account> accounts);
    Task BatchUpdateRoleAsync(List<Guid> customerIds);
}

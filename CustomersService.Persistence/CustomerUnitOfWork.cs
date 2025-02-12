
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence
{
    public class CustomerUnitOfWork(CustomerServiceDbContext context,
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository) : ICustomerUnitOfWork
    {
        public async Task CreateCustomerAsync(Customer customer, Account account)
        {
            Begin();
            await customerRepository.CreateAsync(customer);
            account.Customer = customer;
            await accountRepository.CreateAsync(account);
            Commit();
        }

        public async Task BatchUpdateRoleAsync(List<Guid> customerIds)
        {
            Begin();
            await customerRepository.BatchUpdateRoleAsync(customerIds);
            Commit();
        }

        public async Task SetManualVipAsync(Customer customer, DateTime vipExpirationDate, List<Account> accounts)
        {
            Begin();
            await customerRepository.SetManualVipAsync(customer, vipExpirationDate);
            await accountRepository.ActivateAsync(accounts);
            Commit();
        }

        public void Begin()
        {
            context.BeginTransaction();
        }

        public void Commit()
        {
            context.CommitTransaction();
        }

        public void Rollback()
        {
            context.RollbackTransaction();
        }
    }
}

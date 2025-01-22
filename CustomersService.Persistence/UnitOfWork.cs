
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence
{
    public class UnitOfWork(CustomerServiceDbContext context,
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository): IUnitOfWork
    {
        public ICustomerRepository CustomerRepository
        {
            get { return customerRepository; }
        }

        public IAccountRepository AccountRepository
        {
            get { return accountRepository; }
        }

        public void Begin()
        {
            context.BeginTransaction();
        }

        public void Commit()
        {
            context.SaveChanges();
            context.CommitTransaction();
        }

        public void Rollback()
        {
            context.RollbackTransaction();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}

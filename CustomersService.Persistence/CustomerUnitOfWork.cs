
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence
{
    public class CustomerUnitOfWork(CustomerServiceDbContext context): ICustomerUnitOfWork
    {
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

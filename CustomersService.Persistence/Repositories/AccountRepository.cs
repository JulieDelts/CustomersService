using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;

namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository(CustomerServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
    {
        public async Task ActivateAsync(Account account)
        {
            account.IsDeactivated = false;
            await context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Account account)
        {
            account.IsDeactivated = true;
            await context.SaveChangesAsync();
        }
    }
}

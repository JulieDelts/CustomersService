using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomersService.Persistence.Repositories
{
    public class AccountRepository(CustomerServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
    {
        public async Task ActivateAsync(Account account)
        {
            account.IsDeactivated = false;
            await context.SaveChangesAsync();
        }

        public async Task ActivateAsync(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                account.IsDeactivated = false;
            }

            await context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(Account account)
        {
            account.IsDeactivated = true;
            await context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                account.IsDeactivated = true;
            }

            await context.SaveChangesAsync();
        }
    }
}

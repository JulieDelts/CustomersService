using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Persistence.Repositories;

public class AccountRepository(
    CustomerServiceDbContext context,
    ILogger<AccountRepository> logger) 
    : BaseRepository<Account>(context), IAccountRepository
{
    public async Task ActivateAsync(Account account)
    {
        logger.LogDebug("Activating account {AccountId}", account.Id);
        account.IsDeactivated = false;
        await context.SaveChangesAsync();
        logger.LogDebug("Successfully activated account {AccountId}", account.Id);
    }

    public async Task ActivateAsync(List<Account> accounts)
    {
        logger.LogDebug("Activating multiple accounts");

        foreach (var account in accounts)
        {
            account.IsDeactivated = false;
        }

        await context.SaveChangesAsync();
        logger.LogDebug("Successfully activated multiple accounts");
    }

    public async Task DeactivateAsync(Account account)
    {
        logger.LogDebug("Deactivating account {AccountId}", account.Id);
        account.IsDeactivated = true;
        await context.SaveChangesAsync();
        logger.LogDebug("Successfully deactivated account {AccountId}", account.Id);
    }
}

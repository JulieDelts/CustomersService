
using CustomersService.Application.Exceptions;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Services.ServicesUtils;

public class AccountUtils(
    IAccountRepository accountRepository,
    ILogger<AccountUtils> logger)
{
    public async Task<Account> GetByIdAsync(Guid id)
    {
        logger.LogDebug("Retrieving account with ID {AccountId}", id);

        var accountDTO = await accountRepository.GetByConditionAsync(a => a.Id == id);
        logger.LogTrace("Retrieved account data: {@AccountDTO}", accountDTO);

        if (accountDTO == null)
        {
            logger.LogWarning("Account with id {AccountId} was not found", id);
            throw new EntityNotFoundException($"Account with id {id} was not found.");
        }

        logger.LogDebug("Successfully retrieved account with ID {AccountId}", id);
        return accountDTO;
    }
}

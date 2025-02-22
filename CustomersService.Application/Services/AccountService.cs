using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using CustomersService.Core.IntegrationModels.Responses;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Application.Services;

public class AccountService(
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<AccountService> logger,
        ICommonHttpClient httpClient,
        IOptions<TransactionStoreApiConnectionStrings> options) 
    : IAccountService
{
    private readonly string controllerPath = options.Value?.Accounts;

    public async Task<Guid> CreateAsync(AccountCreationModel accountToCreate)
    {
        logger.LogInformation("Creating account for customer {CustomerId}", accountToCreate.CustomerId);

        var customerDto = await customerUtils.GetByIdAsync(accountToCreate.CustomerId);

        if (customerDto.Role == Role.Admin || customerDto.Role == Role.Unknown)
        {
            logger.LogError("Customer with id {CustomerId} has an invalid role {Role}", accountToCreate.CustomerId, customerDto.Role);
            throw new EntityConflictException($"Role of customer with id {accountToCreate.CustomerId} is not correct.");
        }

        if (customerDto.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", accountToCreate.CustomerId);
            throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} is deactivated.");
        }

        var accountDto = await accountRepository.GetByConditionAsync(a =>
        a.Currency == accountToCreate.Currency && a.CustomerId == accountToCreate.CustomerId);

        if (accountDto != null)
        {
            logger.LogWarning("Customer with id {CustomerId} already has an account with currency {Currency}", accountToCreate.CustomerId, accountToCreate.Currency);
            throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} already has an account with currency {accountToCreate.Currency}.");
        }

        if (customerDto.Role == Role.Regular
            && accountToCreate.Currency != Currency.USD
            && accountToCreate.Currency != Currency.EUR)
        {
            logger.LogError("Customer with role {Role} cannot have an account with currency {Currency}", customerDto.Role, accountToCreate.Currency);
            throw new EntityConflictException($"Customer with role {customerDto.Role} cannot have an account with this currency.");
        }

        var accountToCreateDto = mapper.Map<Account>(accountToCreate);
        accountToCreateDto.Customer = customerDto;

        await accountRepository.CreateAsync(accountToCreateDto);
        logger.LogInformation("Account created successfully with ID {AccountId}", accountToCreateDto.Id);

        return accountToCreateDto.Id;
    }

    public async Task<List<AccountInfoModel>> GetAllByCustomerIdAsync(Guid customerId)
    {
        logger.LogInformation("Retrieving all accounts for customer {CustomerId}", customerId);

        var accountDtos = await accountRepository.GetAllByConditionAsync(a => a.CustomerId == customerId);
        var accounts = mapper.Map<List<AccountInfoModel>>(accountDtos);

        logger.LogInformation("Successfully retrieved {Count} accounts for customer {CustomerId}", accounts.Count, customerId);
        return accounts;
    }

    public async Task<AccountFullInfoModel> GetFullInfoByIdAsync(Guid id, Guid customerId)
    {
        logger.LogInformation("Retrieving full account info for account {AccountId}", id);

        var accountDto = await accountUtils.GetByIdAsync(id);

        if (accountDto.CustomerId != customerId)
        {
            logger.LogWarning("Customers are only allowed to see their own account info {accountId}.", id);
            throw new AuthorizationFailedException("Customers are only allowed to see their own account info.");
        }

        var account = mapper.Map<AccountFullInfoModel>(accountDto);
        var accountBalanceModel = await httpClient.SendGetRequestAsync<BalanceResponse>($"{controllerPath}/{id}/balance");
        account.Balance = accountBalanceModel.Balance;
        logger.LogInformation("Successfully retrieved full account info for account {AccountId}", id);

        return account;
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id, Guid customerId)
    {
        logger.LogInformation("Retrieving transactions for account {AccountId}", id);

        var accountDto = await accountUtils.GetByIdAsync(id);

        if (accountDto.CustomerId != customerId)
        {
            logger.LogWarning("Customers are only allowed to see their own account info {accountId}.", id);
            throw new AuthorizationFailedException("Customers are only allowed to see their own account info.");
        }

        var transactions = await httpClient.SendGetRequestAsync<List<TransactionResponse>>($"{controllerPath}/{id}/transactions");

        logger.LogInformation("Successfully retrieved {Count} transactions for account {AccountId}", transactions.Count, id);
        return transactions;
    }

    public async Task DeactivateAsync(Guid id)
    {
        logger.LogInformation("Deactivating account {AccountId}", id);

        var accountDto = await accountUtils.GetByIdAsync(id);
        if (accountDto.Currency == Currency.RUB)
        {
            logger.LogWarning("Account with currency {Currency} cannot be deactivated", Currency.RUB);
            throw new EntityConflictException($"Account with currency {Currency.RUB} cannot be deactivated.");
        }

        await accountRepository.DeactivateAsync(accountDto);
        logger.LogInformation("Successfully deactivated account {AccountId}", id);
    }

    public async Task ActivateAsync(Guid id)
    {
        logger.LogInformation("Activating account {AccountId}", id);

        var accountDto = await accountUtils.GetByIdAsync(id);
        await accountRepository.ActivateAsync(accountDto);
        logger.LogInformation("Successfully activated account {AccountId}", id);
    }
}

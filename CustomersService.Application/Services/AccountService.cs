
using AutoMapper;
using CustomersService.Application.Exceptions;
using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Models;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;
using CustomersService.Persistence.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomersService.Application.Services;

public class AccountService(
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<AccountService> logger,
        IOptions<TransactionStoreAPIConnectionStrings> options) 
    : IAccountService
{
    private readonly CommonHttpClient _httpClient;

    public AccountService(
        IAccountRepository accountRepository,
        IMapper mapper,
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<AccountService> logger,
         IOptions<TransactionStoreAPIConnectionStrings> options,
        HttpMessageHandler? handler = null): 
        this(accountRepository, mapper, customerUtils, accountUtils, logger, options)
    {
        _httpClient = new(options.Value.TransactionStoreAccounts, handler);
    }

    public async Task<Guid> CreateAsync(AccountCreationModel accountToCreate)
    {
        logger.LogInformation("Creating account for customer {CustomerId}", accountToCreate.CustomerId);
        logger.LogDebug("Account creation data: {@AccountToCreate}", accountToCreate);

        var customerDTO = await customerUtils.GetByIdAsync(accountToCreate.CustomerId);
        logger.LogTrace("Retrieved customer data: {@CustomerDTO}", customerDTO);

        if (customerDTO.Role == Role.Admin || customerDTO.Role == Role.Unknown)
        {
            logger.LogCritical("Customer with id {CustomerId} has an invalid role {Role}", accountToCreate.CustomerId, customerDTO.Role);
            throw new EntityConflictException($"Role of customer with id {accountToCreate.CustomerId} is not correct.");
        }

        if (customerDTO.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", accountToCreate.CustomerId);
            throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} is deactivated.");
        }

        var accountDTO = await accountRepository.GetByConditionAsync(a =>
        a.Currency == accountToCreate.Currency && a.CustomerId == accountToCreate.CustomerId);
        logger.LogTrace("Checked for existing account with same currency: {@AccountDTO}", accountDTO);

        if (accountDTO != null)
        {
            logger.LogWarning("Customer with id {CustomerId} already has an account with currency {Currency}", accountToCreate.CustomerId, accountToCreate.Currency);
            throw new EntityConflictException($"Customer with id {accountToCreate.CustomerId} already has an account with currency {accountToCreate.Currency}.");
        }

        if (customerDTO.Role == Role.Regular
            && accountToCreate.Currency != Currency.USD
            && accountToCreate.Currency != Currency.EUR)
        {
            logger.LogError("Customer with role {Role} cannot have an account with currency {Currency}", customerDTO.Role, accountToCreate.Currency);
            throw new EntityConflictException($"Customer with role {customerDTO.Role} cannot have an account with this currency.");
        }

        var accountToCreateDTO = mapper.Map<Account>(accountToCreate);
        accountToCreateDTO.Customer = customerDTO;
        logger.LogTrace("Mapped AccountCreationModel to Account: {@AccountToCreateDTO}", accountToCreateDTO);

        await accountRepository.CreateAsync(accountToCreateDTO);
        logger.LogInformation("Account created successfully with ID {AccountId}", accountToCreateDTO.Id);

        return accountToCreateDTO.Id;
    }

    public async Task<List<AccountInfoModel>> GetAllByCustomerIdAsync(Guid customerId)
    {
        logger.LogInformation("Retrieving all accounts for customer {CustomerId}", customerId);

        var accountDTOs = await accountRepository.GetAllByConditionAsync(a => a.CustomerId == customerId);
        logger.LogTrace("Retrieved accounts: {@AccountDTOs}", accountDTOs);

        var accounts = mapper.Map<List<AccountInfoModel>>(accountDTOs);
        logger.LogTrace("Mapped AccountDTOs to AccountInfoModels: {@Accounts}", accounts);

        logger.LogInformation("Successfully retrieved {Count} accounts for customer {CustomerId}", accounts.Count, customerId);
        return accounts;
    }

    public async Task<AccountFullInfoModel> GetFullInfoByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving full account info for account {AccountId}", id);

        var accountDTO = await accountUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved account data: {@AccountDTO}", accountDTO);

        var account = mapper.Map<AccountFullInfoModel>(accountDTO);
        logger.LogTrace("Mapped AccountDTO to AccountFullInfoModel: {@Account}", account);

        var accountBalanceModel = await _httpClient.SendGetRequestAsync<BalanceResponse>($"/{id}/balance");
        logger.LogTrace("Retrieved account balance: {@AccountBalanceModel}", accountBalanceModel);

        account.Balance = accountBalanceModel.Balance;
        logger.LogInformation("Successfully retrieved full account info for account {AccountId}", id);

        return account;
    }

    public async Task<List<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving transactions for account {AccountId}", id);

        var transactions = await _httpClient.SendGetRequestAsync<List<TransactionResponse>>($"/{id}/transactions");
        logger.LogTrace("Retrieved transactions: {@Transactions}", transactions);

        logger.LogInformation("Successfully retrieved {Count} transactions for account {AccountId}", transactions.Count, id);
        return transactions;
    }

    public async Task DeactivateAsync(Guid id)
    {
        logger.LogInformation("Deactivating account {AccountId}", id);

        var accountDTO = await accountUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved account data: {@AccountDTO}", accountDTO);

        if (accountDTO.Currency == Currency.RUB)
        {
            logger.LogWarning("Account with currency {Currency} cannot be deactivated", Currency.RUB);
            throw new EntityConflictException($"Account with currency {Currency.RUB} cannot be deactivated.");
        }

        await accountRepository.DeactivateAsync(accountDTO);
        logger.LogInformation("Successfully deactivated account {AccountId}", id);
    }

    public async Task ActivateAsync(Guid id)
    {
        logger.LogInformation("Activating account {AccountId}", id);

        var accountDTO = await accountUtils.GetByIdAsync(id);
        logger.LogTrace("Retrieved account data: {@AccountDTO}", accountDTO);

        await accountRepository.ActivateAsync(accountDTO);
        logger.LogInformation("Successfully activated account {AccountId}", id);
    }
}


using CustomersService.Application.Exceptions;
using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Services;

public class TransactionService(
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<TransactionService> logger) 
    : ITransactionService
{
    private readonly CommonHttpClient _httpClient;

    public TransactionService(
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<TransactionService> logger,
        ILogger<CommonHttpClient> commonHttpClientLogger,
        HttpMessageHandler? handler = null):
        this (customerUtils, accountUtils,logger)
    {
        _httpClient = new CommonHttpClient("http://194.147.90.249:9091/api/v1/transactions", commonHttpClientLogger,handler);
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving transaction with ID {TransactionId}", id);
        var transaction = await _httpClient.SendGetRequestAsync<TransactionResponse>($"/{id}");
        logger.LogTrace("Retrieved transaction: {@Transaction}", transaction);
        return transaction;
    }

    public async Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, TransactionType transactionType)
    {
        logger.LogInformation("Creating simple transaction of type {TransactionType} for account {AccountId}", transactionType, requestModel.AccountId);
        logger.LogTrace("Transaction request data: {@RequestModel}", requestModel);

        string path;
        if (transactionType == TransactionType.Deposit)
            path = "/deposit";
        else if (transactionType == TransactionType.Withdrawal)
            path = "/withdraw";
        else
        {
            logger.LogWarning("Transaction type {TransactionType} is not supported", transactionType);
            throw new EntityConflictException("Transaction type is not supported.");
        }

        var account = await accountUtils.GetByIdAsync(requestModel.AccountId);
        logger.LogTrace("Retrieved account data: {@Account}", account);

        if (account.IsDeactivated)
        {
            logger.LogWarning("Account with id {AccountId} is deactivated", account.Id);
            throw new EntityConflictException($"Account with id {account.Id} is deactivated.");
        }

        if (account.Currency != Currency.RUB && account.Currency != Currency.USD)
        {
            logger.LogWarning("Deposit and withdraw transactions are only allowed for accounts with currencies {CurrencyRUB} and {CurrencyUSD}", Currency.RUB, Currency.USD);
            throw new EntityConflictException($"Deposit and withdraw transactions are only allowed for accounts with currencies {Currency.RUB}, {Currency.USD}.");
        }

        var customer = await customerUtils.GetByIdAsync(account.CustomerId);
        logger.LogTrace("Retrieved customer data: {@Customer}", customer);

        if (customer.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", customer.Id);
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");
        }

        var transactionId = await _httpClient.SendPostRequestAsync<CreateTransactionRequest, Guid>(path, requestModel);
        logger.LogInformation("Successfully created transaction with ID {TransactionId}", transactionId);
        return transactionId;
    }

    public async Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel)
    {
        logger.LogInformation("Creating transfer transaction from account {FromAccountId} to account {ToAccountId}", requestModel.FromAccountId, requestModel.ToAccountId);
        logger.LogTrace("Transfer transaction request data: {@RequestModel}", requestModel);

        var vipAccounts = new List<Currency>()
        { Currency.JPY, Currency.CNY, Currency.RSD, Currency.BGN, Currency.ARS };

        var fromAccount = await accountUtils.GetByIdAsync(requestModel.FromAccountId);
        logger.LogTrace("Retrieved from account data: {@FromAccount}", fromAccount);

        if (fromAccount.IsDeactivated && !vipAccounts.Contains(fromAccount.Currency))
        {
            logger.LogWarning("Account with id {FromAccountId} is deactivated", fromAccount.Id);
            throw new EntityConflictException($"Account with id {fromAccount.Id} is deactivated.");
        }

        var toAccount = await accountUtils.GetByIdAsync(requestModel.ToAccountId);
        logger.LogTrace("Retrieved to account data: {@ToAccount}", toAccount);

        if (toAccount.IsDeactivated)
        {
            logger.LogWarning("Account with id {ToAccountId} is deactivated", toAccount.Id);
            throw new EntityConflictException($"Account with id {toAccount.Id} is deactivated.");
        }

        if (fromAccount.CustomerId != toAccount.CustomerId)
        {
            logger.LogWarning("Accounts must belong to the same customer");
            throw new EntityConflictException("Accounts must belong to the same customer.");
        }

        if (fromAccount.IsDeactivated
            && vipAccounts.Contains(fromAccount.Currency)
            && toAccount.Currency != Currency.RUB)
        {
            logger.LogWarning("Transfer is allowed only to the account with currency {CurrencyRUB}", Currency.RUB);
            throw new EntityConflictException($"Transfer is allowed only to the account with currency {Currency.RUB}.");
        }

        var customer = await customerUtils.GetByIdAsync(fromAccount.CustomerId);
        logger.LogTrace("Retrieved customer data: {@Customer}", customer);

        if (customer.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", customer.Id);
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");
        }

        var transactionIds = await _httpClient.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>("/transfer", requestModel);
        logger.LogInformation("Successfully created transfer transactions with IDs {TransactionIds}", transactionIds);
        return transactionIds;
    }
} 


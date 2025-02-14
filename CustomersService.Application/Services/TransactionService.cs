
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
        HttpMessageHandler? handler = null) :
        this(customerUtils, accountUtils, logger)
    {
        _httpClient = new CommonHttpClient("http://194.147.90.249:9091/api/v1/transactions", handler);
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
        string path;
        if (transactionType == TransactionType.Deposit)
            path = "/deposit";
        else if (transactionType == TransactionType.Withdrawal)
            path = "/withdraw";
        else throw new EntityConflictException("TransactionType is not correct.");

        logger.LogInformation("Creating simple transaction for account {AccountId}", requestModel.AccountId);
        logger.LogTrace("Transaction request data: {@RequestModel}", requestModel);

        await ValidateSimpleTransactionRequestAsync(requestModel);

        var transactionId = await _httpClient.SendPostRequestAsync<CreateTransactionRequest, Guid>(path, requestModel);
        logger.LogInformation("Successfully created transaction with ID {TransactionId}", transactionId);
        return transactionId;
    }

    public async Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel)
    {
        logger.LogInformation("Creating transfer transaction from account {FromAccountId} to account {ToAccountId}", requestModel.FromAccountId, requestModel.ToAccountId);
        logger.LogTrace("Transfer transaction request data: {@RequestModel}", requestModel);

        await ValidateTransferTransactionRequestAsync(requestModel);

        var transactionIds = await _httpClient.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>("/transfer", requestModel);
        logger.LogInformation("Successfully created transfer transactions with IDs {TransactionIds}", transactionIds);
        return transactionIds;
    }

    private async Task ValidateSimpleTransactionRequestAsync(CreateTransactionRequest requestModel)
    {
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
    }

    private async Task ValidateTransferTransactionRequestAsync(CreateTransferTransactionRequest requestModel)
    {
        var fromAccount = await accountUtils.GetByIdAsync(requestModel.FromAccountId);
        logger.LogTrace("Retrieved from account data: {@FromAccount}", fromAccount);

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
            && toAccount.Currency != Currency.RUB)
        {
            logger.LogWarning("Transfer from deactivated accounts is allowed only to the account with currency {CurrencyRUB}", Currency.RUB);
            throw new EntityConflictException($"Transfer from deactivated accounts is allowed only to the account with currency {Currency.RUB}.");
        }

        var customer = await customerUtils.GetByIdAsync(fromAccount.CustomerId);
        logger.LogTrace("Retrieved customer data: {@Customer}", customer);

        if (customer.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", customer.Id);
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");
        }
    }
}


using CustomersService.Application.Exceptions;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using CustomersService.Core.IntegrationModels.Requests;
using CustomersService.Core.IntegrationModels.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Application.Services;

public class TransactionService(
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        ILogger<TransactionService> logger,
        ICommonHttpClient httpClient,
        IOptions<TransactionStoreApiConnectionStrings> options)
    : ITransactionService
{
    private readonly string controllerPath = options.Value?.Transactions;

    public async Task<TransactionResponse> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Retrieving transaction with ID {TransactionId}", id);
        var transaction = await httpClient.SendGetRequestAsync<TransactionResponse>($"{controllerPath}/{id}");
        return transaction;
    }

    public async Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, Guid customerId, TransactionType transactionType)
    {
        string path;
        if (transactionType == TransactionType.Deposit)
            path = $"{controllerPath}/deposit";
        else if (transactionType == TransactionType.Withdrawal)
            path = $"{controllerPath}/withdraw";
        else throw new EntityConflictException("TransactionType is not correct.");

        logger.LogInformation("Creating simple transaction for account {AccountId}", requestModel.AccountId);

        await ValidateSimpleTransactionRequestAsync(requestModel, customerId);

        var transactionId = await httpClient.SendPostRequestAsync<CreateTransactionRequest, Guid>(path, requestModel);
        logger.LogInformation("Successfully created transaction with ID {TransactionId}", transactionId);
        return transactionId;
    }

    public async Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel, Guid customerId)
    {
        logger.LogInformation("Creating transfer transaction from account {FromAccountId} to account {ToAccountId}", requestModel.FromAccountId, requestModel.ToAccountId);

        await ValidateTransferTransactionRequestAsync(requestModel, customerId);

        var transactionIds = await httpClient.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>($"{controllerPath}/transfer", requestModel);
        logger.LogInformation("Successfully created transfer transactions with IDs {TransactionIds}", transactionIds);
        return transactionIds;
    }

    private async Task ValidateSimpleTransactionRequestAsync(CreateTransactionRequest requestModel, Guid customerId)
    {
        var account = await accountUtils.GetByIdAsync(requestModel.AccountId);

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

        if(account.CustomerId != customerId)
        {
            logger.LogWarning("Customers are only allowed to create transactions for their own accounts. Account Id {accountId}", account.Id);
            throw new AuthorizationFailedException("Customers are only allowed to create transactions for their own accounts.");
        }

        var customer = await customerUtils.GetByIdAsync(account.CustomerId);

        if (customer.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", customer.Id);
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");
        }
    }

    private async Task ValidateTransferTransactionRequestAsync(CreateTransferTransactionRequest requestModel, Guid customerId)
    {
        var fromAccount = await accountUtils.GetByIdAsync(requestModel.FromAccountId);

        var toAccount = await accountUtils.GetByIdAsync(requestModel.ToAccountId);

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

        if (fromAccount.CustomerId != customerId)
        {
            logger.LogWarning("Customers are only allowed to create transactions for their own accounts. Account Ids {fromAccountId}, {toAccountId}", requestModel.FromAccountId, requestModel.ToAccountId);
            throw new AuthorizationFailedException("Customers are only allowed to create transactions for their own accounts.");
        }

        if (fromAccount.IsDeactivated
            && toAccount.Currency != Currency.RUB)
        {
            logger.LogWarning("Transfer from deactivated accounts is allowed only to the account with currency {CurrencyRUB}", Currency.RUB);
            throw new EntityConflictException($"Transfer from deactivated accounts is allowed only to the account with currency {Currency.RUB}.");
        }

        var customer = await customerUtils.GetByIdAsync(fromAccount.CustomerId);

        if (customer.IsDeactivated)
        {
            logger.LogWarning("Customer with id {CustomerId} is deactivated", customer.Id);
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");
        }

        requestModel.ToCurrency = toAccount.Currency;
        requestModel.FromCurrency = fromAccount.Currency;
    }
}


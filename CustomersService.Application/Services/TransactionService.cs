
using CustomersService.Application.Exceptions;
using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;

namespace CustomersService.Application.Services;

public class TransactionService(
    CustomerUtils customerUtils,
    AccountUtils accountUtils) : ITransactionService
{
    private readonly CommonHttpClient _httpClient;

    public TransactionService(
        CustomerUtils customerUtils,
        AccountUtils accountUtils,
        HttpMessageHandler? handler = null):
        this (customerUtils, accountUtils)
    {
        _httpClient = new CommonHttpClient("http://194.147.90.249:9091/api/v1/transactions", handler);
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id)
    {
         return await _httpClient.SendGetRequestAsync<TransactionResponse>($"/{id}");
    }

    public async Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, TransactionType transactionType) 
    {  
        string path;
        if (transactionType == TransactionType.Deposit)
            path = "/deposit";
        else if (transactionType == TransactionType.Withdrawal)
            path = "/withdraw";
        else throw new EntityConflictException("Transaction type is not supported.");

        var account = await accountUtils.GetByIdAsync(requestModel.AccountId);
        if (account.IsDeactivated)
            throw new EntityConflictException($"Account with id {account.Id} is deactivated.");

        if (account.Currency != Currency.RUB && account.Currency != Currency.USD)
            throw new EntityConflictException($"Deposit and withdraw transactions are only allowed for accounts with currencies {Currency.RUB}, {Currency.USD}.");

        var customer = await customerUtils.GetByIdAsync(account.CustomerId);
        if (customer.IsDeactivated)
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");

        return await _httpClient.SendPostRequestAsync<CreateTransactionRequest, Guid>(path, requestModel);
    }

    public async Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel)
    {
        var vipAccounts = new List<Currency>()
        { Currency.JPY, Currency.CNY, Currency.RSD, Currency.BGN, Currency.ARS };

        var fromAccount = await accountUtils.GetByIdAsync(requestModel.FromAccountId);

        if (fromAccount.IsDeactivated && !vipAccounts.Contains(fromAccount.Currency))
            throw new EntityConflictException($"Account with id {fromAccount.Id} is deactivated.");

        var toAccount = await accountUtils.GetByIdAsync(requestModel.ToAccountId);

        if (toAccount.IsDeactivated)
            throw new EntityConflictException($"Account with id {toAccount.Id} is deactivated.");

        if (fromAccount.CustomerId != toAccount.CustomerId)
            throw new EntityConflictException("Accounts must belong to the same customer.");

        if(fromAccount.IsDeactivated 
            && vipAccounts.Contains(fromAccount.Currency)
            && toAccount.Currency != Currency.RUB)
            throw new EntityConflictException($"Transfer is allowed only to the account with currency {Currency.RUB}.");

        var customer = await customerUtils.GetByIdAsync(fromAccount.CustomerId);
        if (customer.IsDeactivated)
            throw new EntityConflictException($"Customer with id {customer.Id} is deactivated.");

        return await _httpClient.SendPostRequestAsync<CreateTransferTransactionRequest, List<Guid>>("/transfer", requestModel);
    }
} 


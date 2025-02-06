
using CustomersService.Application.Exceptions;
using CustomersService.Application.Intefaces;
using CustomersService.Application.Integrations;
using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;

namespace CustomersService.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly CommonHttpClient _httpClient;

    public TransactionService()
    {
        _httpClient = new CommonHttpClient("api/v1/transactions");
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

        return await _httpClient.SendPostRequestAsync<CreateTransactionRequest, Guid>(path, requestModel);
    }

    public async Task<IEnumerable<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel)
    {
       return await _httpClient.SendPostRequestAsync<CreateTransferTransactionRequest, IEnumerable<Guid>>("/transfer", requestModel);
    }
} 


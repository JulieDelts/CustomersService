using CustomersService.Application.Models;
using CustomersService.Core.IntegrationModels.Requests;
using CustomersService.Core.IntegrationModels.Responses;
using CustomersService.Core.Enum;

namespace CustomersService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, Guid customerId, TransactionType transactionType);
        Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel, Guid customerId);
        Task<TransactionResponse> GetByIdAsync(Guid id);
    }
}

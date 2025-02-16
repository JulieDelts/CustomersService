using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
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

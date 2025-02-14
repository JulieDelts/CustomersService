using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;

namespace CustomersService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, TransactionType transactionType);
        Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel);
        Task<TransactionResponse> GetByIdAsync(Guid id);
    }
}

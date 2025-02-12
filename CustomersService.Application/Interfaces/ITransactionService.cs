using CustomersService.Core.DTOs.Requests;
using CustomersService.Core.DTOs.Responses;
using CustomersService.Core.Enum;

namespace CustomersService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Guid> CreateDepositTransactionAsync(CreateTransactionRequest requestModel);
        Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel);
        Task<Guid> CreateWithdrawTransactionAsync(CreateTransactionRequest requestModel);
        Task<TransactionResponse> GetByIdAsync(Guid id);
    }
}

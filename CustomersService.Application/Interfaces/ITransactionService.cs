using CustomersService.Core.IntegrationModels.Requests;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Application.Interfaces;

public interface ITransactionService
{
    Task<Guid> CreateSimpleTransactionAsync(CreateTransactionRequest requestModel, Guid customerId, TransactionType transactionType);
    Task<List<Guid>> CreateTransferTransactionAsync(CreateTransferTransactionRequest requestModel, Guid customerId);
}

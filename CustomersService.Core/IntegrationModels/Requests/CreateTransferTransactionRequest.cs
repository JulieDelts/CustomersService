using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Core.IntegrationModels.Requests;

public class CreateTransferTransactionRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public Currency FromCurrency { get; set; }
    public Currency ToCurrency { get; set; }
}

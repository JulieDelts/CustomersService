namespace CustomersService.Core.IntegrationModels.Requests;

public class CreateTransactionRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}

namespace CustomersService.Presentation.Models.Requests;

public class TransactionCreateRequest
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}

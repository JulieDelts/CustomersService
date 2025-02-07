using CustomersService.Core.Enum;

namespace CustomersService.Core.DTOs.Requests;

public class CreateTransferTransactionRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public Currency FromCurrency { get; set; }
    public Currency ToCurrency { get; set; }
}

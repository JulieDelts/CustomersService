using CustomersService.Core.Enum;

namespace CustomersService.Core.DTOs.Requests;

public class CreateTransferTransactionRequest
{
    public Guid FromAccountID { get; set; }
    public Guid ToAccountID { get; set; }
    public decimal Amount { get; set; }
    public Currency FromCurrency { get; set; }
    public Currency ToCurrency { get; set; }
}

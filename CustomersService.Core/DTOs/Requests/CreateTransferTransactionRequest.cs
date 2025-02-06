using CustomersService.Core.Enum;

namespace CustomersService.Core.DTOs.Requests;

public class CreateTransferTransactionRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public CurrencyType FromCurrency { get; set; }
    public CurrencyType ToCurrency { get; set; }
}

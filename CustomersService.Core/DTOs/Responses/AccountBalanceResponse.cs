namespace CustomersService.Core.DTOs.Responses;

public class AccountBalanceResponse
{
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
}

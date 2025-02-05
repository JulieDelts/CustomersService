using CustomersService.Core.Enum;
using System.Text.Json.Serialization;


namespace CustomersService.Core.DTOs.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType TransactionType { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TransactionResponse? RelatedTransaction { get; set; }
}

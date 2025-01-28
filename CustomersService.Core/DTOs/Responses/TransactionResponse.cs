using CustomersService.Core.Enum;
using System.Text.Json.Serialization;


namespace CustomersService.Core.DTOs.Responses;

public class TransactionResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("accountId")]
    public Guid AccountId { get; set; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    [JsonPropertyName("type")]
    public TransactionType TransactionType { get; set; }
    [JsonPropertyName("relatedTransaction")]
    public TransactionResponse? RelatedTransaction { get; set; }
}

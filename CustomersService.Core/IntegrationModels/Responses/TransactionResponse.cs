using System.Text.Json.Serialization;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Core.IntegrationModels.Responses;

public class TransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TransactionResponse? RelatedTransaction { get; set; }
}

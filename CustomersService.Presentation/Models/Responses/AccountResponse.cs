using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Presentation.Models.Responses;

public class AccountResponse
{
    public Guid Id { get; set; }
    public Currency Currency { get; set; }
    public bool IsDeactivated { get; set; } 
}

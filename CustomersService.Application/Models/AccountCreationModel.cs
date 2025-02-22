using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Application.Models;

public class AccountCreationModel
{
    public Guid CustomerId { get; set; }
    public Currency Currency { get; set; }
}

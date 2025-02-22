using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Persistence.Entities;

public class Account : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime DateCreated { get; set; }
    public Currency Currency { get; set; }
    public Customer Customer { get; set; }
}

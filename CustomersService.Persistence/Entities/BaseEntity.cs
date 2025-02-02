
namespace CustomersService.Persistence.Entities;
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsDeactivated { get; set; }
}

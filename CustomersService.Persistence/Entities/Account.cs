using CustomersService.Core.Enum;

namespace CustomersService.Persistence.Entities
{
    public class Account : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public CurrencyType Currency { get; set; }
        public Customer Customer { get; set; }
    }
}

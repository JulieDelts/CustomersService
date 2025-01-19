using CustomersService.Core.Enum;

namespace CustomersService.Persistence.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public Currency Currency { get; set; }
        public bool IsDeactivated { get; set; }
    }
}

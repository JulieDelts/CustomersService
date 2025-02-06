using CustomersService.Core.Enum;

namespace CustomersService.Application.Models
{
    public class AccountFullInfoModel
    {
        public Guid Id { get; set; }
        public bool IsDeactivated { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateCreated { get; set; }
        public CurrencyType Currency { get; set; }
        public Guid CustomerId { get; set; }
    }
}

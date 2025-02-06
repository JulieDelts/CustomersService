
using CustomersService.Core.Enum;

namespace CustomersService.Application.Models
{
    public class AccountInfoModel
    {
        public Guid Id { get; set; }
        public bool IsDeactivated { get; set; }
        public CurrencyType Currency { get; set; }
        public Guid CustomerId { get; set; }
    }
}

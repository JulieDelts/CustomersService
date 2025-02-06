
using CustomersService.Core.Enum;

namespace CustomersService.Application.Models
{
    public class AccountCreationModel
    {
        public Guid CustomerId { get; set; }
        public CurrencyType Currency { get; set; }
    }
}

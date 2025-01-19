using CustomersService.Core.Enum;

namespace CustomersService.Presentation.Models.Requests
{
    public class AccountAddRequest
    {
        public Guid CustomerId { get; set; }
        public Currency Currency { get; set; }
    }
}

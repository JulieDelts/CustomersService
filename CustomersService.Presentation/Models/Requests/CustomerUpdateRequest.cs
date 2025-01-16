using CustomersService.Core.Enum;
using CustomersService.Persistence.Entities;

namespace CustomersService.Presentation.Models.Requests
{
    public class CustomerUpdateRequest
    {
        public CustomerRole Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

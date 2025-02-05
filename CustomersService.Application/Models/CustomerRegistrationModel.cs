
namespace CustomersService.Application.Models
{
    public class CustomerRegistrationModel
    {
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateOnly BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

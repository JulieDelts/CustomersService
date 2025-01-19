using CustomersService.Core.Enum;

namespace CustomersService.Persistence.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeactivated { get; set; }
    }
}

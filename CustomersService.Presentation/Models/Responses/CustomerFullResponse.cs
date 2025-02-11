using CustomersService.Core.Enum;

namespace CustomersService.Presentation.Models.Responses
{
    public class CustomerFullResponse
    {
        public Guid Id { get; set; }
        public bool IsDeactivated { get; set; }
        public Role Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateOnly BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

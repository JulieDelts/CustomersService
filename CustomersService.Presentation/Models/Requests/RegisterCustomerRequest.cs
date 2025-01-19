namespace CustomersService.Presentation.Models.Requests
{
    public class RegisterCustomerRequest
    {
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

namespace CustomersService.Presentation.Models.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; } 
    }
}

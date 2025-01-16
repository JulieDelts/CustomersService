namespace CustomersService.Presentation.Models.Requests
{
    public class AccountAddRequest
    {
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } = "Is Actived";
    }
}

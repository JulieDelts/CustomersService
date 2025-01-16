namespace CustomersService.Persistence.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Currency { get; set; }
        public bool Status { get; set; }
    }
}

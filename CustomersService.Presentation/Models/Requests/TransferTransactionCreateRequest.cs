
namespace CustomersService.Presentation.Models.Requests
{
    public class TransferTransactionCreateRequest
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}

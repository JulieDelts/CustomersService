namespace CustomersService.Presentation.Models.Requests;

public class CustomerUpdateRequest
{
    public string Phone { get; set; }
    public string Address { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

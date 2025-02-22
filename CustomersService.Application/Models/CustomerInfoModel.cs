namespace CustomersService.Application.Models;

public class CustomerInfoModel
{
    public Guid Id { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateOnly BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

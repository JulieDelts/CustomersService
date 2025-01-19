namespace CustomersService.Presentation.Models.Requests
{
    public class PasswordUpdateRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

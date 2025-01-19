using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(model => model.Email).NotNull().EmailAddress();
            RuleFor(model => model.Password).NotNull().Length(8, 15);
        }
    }
}

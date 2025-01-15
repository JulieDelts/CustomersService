using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(model => model.Email).EmailAddress().WithMessage("Этот email не email");
            RuleFor(model => model.Password).MinimumLength(8);
        }
    }
}

using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class PasswordUpdateRequestValidator: AbstractValidator<PasswordUpdateRequest>
    {
        public PasswordUpdateRequestValidator() 
        {
            RuleFor(model => model.NewPassword).NotNull().Length(8, 15);
            RuleFor(model => model.CurrentPassword).NotNull().Length(8, 15);
        }
    }
}

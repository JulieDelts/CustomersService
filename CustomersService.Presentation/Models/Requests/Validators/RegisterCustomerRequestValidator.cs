using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerRequestValidator()
        {
            RuleFor(model => model.Phone).Length(11);
            RuleFor(model => model.Password).MinimumLength(8);
            RuleFor(model => model.Email).EmailAddress().MaximumLength(50).MinimumLength(3);
            RuleFor(model => model.BirthDate).NotEmpty();
            RuleFor(model => model.FirstName).NotEmpty();
            RuleFor(model => model.LastName).NotEmpty();
            RuleFor(model => model.Address).NotEmpty();
        }
    }
}

using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
    {
        public RegisterCustomerRequestValidator()
        {
            RuleFor(model => model.Email).NotNull().EmailAddress();
            RuleFor(model => model.Password).NotNull().Length(8, 15);
            RuleFor(model => model.BirthDate).NotEmpty();
            RuleFor(model => model.Phone).NotEmpty().Length(11);
            RuleFor(model => model.Address).NotEmpty().Length(10, 100);
            RuleFor(model => model.FirstName).NotEmpty().Length(1, 20);
            RuleFor(model => model.LastName).NotEmpty().Length(1, 20);
        }
    }
}

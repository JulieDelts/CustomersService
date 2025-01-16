using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class CustomerUpdateRequestValidator : AbstractValidator<CustomerUpdateRequest>
    {
        public CustomerUpdateRequestValidator() 
        {
            RuleFor(model => model.Role).NotEmpty();
            RuleFor(model => model.Phone).MinimumLength(11);
            RuleFor(model => model.Address).NotEmpty();
            RuleFor(model => model.Email).EmailAddress();
            RuleFor(model => model.Password).MinimumLength(8);
            RuleFor(model => model.FirstName).MinimumLength(2);
            RuleFor(model => model.LastName).MinimumLength(2);
        }
    }
}

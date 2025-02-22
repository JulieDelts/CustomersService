using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators;

public class CustomerUpdateRequestValidator : AbstractValidator<CustomerUpdateRequest>
{
    public CustomerUpdateRequestValidator() 
    {
        RuleFor(model => model.Phone).NotEmpty().Length(11);
        RuleFor(model => model.Address).NotEmpty().Length(10, 100);
        RuleFor(model => model.FirstName).NotEmpty().Length(1, 20);
        RuleFor(model => model.LastName).NotEmpty().Length(1, 20);
    }
}

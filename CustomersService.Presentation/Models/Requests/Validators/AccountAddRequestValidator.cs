using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class AccountAddRequestValidator : AbstractValidator<AccountAddRequest>
    {
        public AccountAddRequestValidator()
        {
            RuleFor(model => model.Id).NotEmpty();
            RuleFor(model => model.CustomerId).NotEmpty();
            RuleFor(model => model.DateCreated).NotEmpty();
            RuleFor(model => model.Status).NotEmpty();
            RuleFor(model => model.Currency).NotEmpty();
        }
    }
}

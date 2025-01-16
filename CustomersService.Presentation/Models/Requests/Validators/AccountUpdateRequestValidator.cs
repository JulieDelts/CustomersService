using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpdateRequestValidator()
        {
            RuleFor(model => model.Status).NotEmpty();
            RuleFor(model => model.Currency).NotEmpty();
        }
    }
}

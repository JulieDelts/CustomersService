using FluentValidation;
using System.Reflection;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpdateRequestValidator()
        {
            RuleFor(model => model.Status).NotEmpty();
        }
    }
}

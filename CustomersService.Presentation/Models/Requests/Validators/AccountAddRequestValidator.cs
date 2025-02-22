using FluentValidation;
using MYPBackendMicroserviceIntegrations.Enums;

namespace CustomersService.Presentation.Models.Requests.Validators;

public class AccountAddRequestValidator : AbstractValidator<AccountAddRequest>
{
    public AccountAddRequestValidator()
    {
        RuleFor(model => model.CustomerId).NotEmpty();
        RuleFor(model => model.Currency).NotEmpty().IsInEnum().NotEqual(Currency.Unknown);
    }
}

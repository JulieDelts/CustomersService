using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class TransactionCreateRequestValidator: AbstractValidator<TransactionCreateRequest>
    {
        public TransactionCreateRequestValidator() 
        {
            RuleFor(model => model.AccountId).NotEmpty();
            RuleFor(model => model.Amount).NotEmpty().GreaterThan(0);
        }
    }
}

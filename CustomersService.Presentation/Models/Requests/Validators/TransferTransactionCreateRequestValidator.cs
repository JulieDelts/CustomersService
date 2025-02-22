using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators;

public class TransferTransactionCreateRequestValidator: AbstractValidator<TransferTransactionCreateRequest>
{
    public TransferTransactionCreateRequestValidator() 
    {
        RuleFor(model => model.ToAccountId).NotEmpty();
        RuleFor(model => model.FromAccountId).NotEmpty();
        RuleFor(model => model.Amount).NotEmpty().GreaterThan(0);
    }
}

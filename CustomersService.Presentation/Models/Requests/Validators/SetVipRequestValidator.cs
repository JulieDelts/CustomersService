using FluentValidation;

namespace CustomersService.Presentation.Models.Requests.Validators
{
    public class SetVipRequestValidator: AbstractValidator<SetVipRequest>
    {
        public SetVipRequestValidator() 
        {
            RuleFor(model => model.VipExpirationDate).NotEmpty().GreaterThan(DateTime.Now);
        }
    }
}

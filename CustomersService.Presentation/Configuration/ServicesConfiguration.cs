using CustomersService.Presentation.Models.Requests.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CustomersService.Presentation.Configuration
{
    public static class ServicesConfiguration
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        }
    }
}

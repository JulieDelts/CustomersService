using CustomersService.Core;
using CustomersService.Presentation.Models.Requests.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;

namespace CustomersService.Presentation.Configuration
{
    public static class ServicesConfiguration
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddOptions<TransactionStoreAPIConnectionStrings>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    var section = configuration.GetSection("TransactionStoreApiConnectionStrings");

                    options.BaseUrl = section.GetValue<string>("BaseUrl") ?? string.Empty;
                    options.Accounts = section.GetSection("Endpoints").GetValue<string>("Accounts") ?? string.Empty;
                    options.Transactions = section.GetSection("Endpoints").GetValue<string>("Transactions") ?? string.Empty;
                });
            services.AddOptions<AuthConfigOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                {
                    var section = configuration.GetSection("AuthConfigOptions");
                    options.Audience = section.GetValue<string>("Audience") ?? string.Empty;
                    options.Issuer = section.GetValue<string>("Issuer") ?? string.Empty;
                    options.Key = section.GetValue<string>("Key") ?? string.Empty;

                });
            services.AddAuth(services.BuildServiceProvider().GetRequiredService<IOptions<AuthConfigOptions>>());
        }
    }
}

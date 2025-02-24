using CustomersService.Core;
using CustomersService.Presentation.Models.Requests.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;

namespace CustomersService.Presentation.Configuration;

public static class ServicesConfiguration
{
    public static void AddPresentationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
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

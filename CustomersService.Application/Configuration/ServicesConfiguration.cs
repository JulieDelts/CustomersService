using CustomersService.Application.Interfaces;
using CustomersService.Application.Mappings;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using Microsoft.Extensions.DependencyInjection;

namespace CustomersService.Application.Configuration
{
    public static class ServicesConfiguration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<AccountUtils>();
            services.AddScoped<CustomerUtils>();
        }
    }
}

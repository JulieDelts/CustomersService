using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomersService.Application.Configuration;

public static class ServicesConfiguration
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<AccountUtils>();
        services.AddScoped<CustomerUtils>();
        services.AddHttpClient<ICommonHttpClient, CommonHttpClient>((serviceProvider, client) =>
         {
            var options = serviceProvider.GetRequiredService<IOptions<TransactionStoreApiConnectionStrings>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl);
            client.Timeout = new TimeSpan(0, 5, 0);
         });
    }
}

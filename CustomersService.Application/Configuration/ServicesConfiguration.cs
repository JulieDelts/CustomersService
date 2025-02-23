using CustomersService.Application.Integrations;
using CustomersService.Application.Interfaces;
using CustomersService.Application.Services;
using CustomersService.Application.Services.ServicesUtils;
using CustomersService.Core;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
        services.AddScoped<RabbitMqPublishUtils>();
        services.AddOptions<TransactionStoreApiConnectionStrings>()
         .Configure<IConfiguration>((options, configuration) =>
         {
             var section = configuration.GetSection("TransactionStoreApiConnectionStrings");
             options.BaseUrl = section.GetValue<string>("BaseUrl") ?? string.Empty;
             options.Accounts = section.GetSection("Endpoints").GetValue<string>("Accounts") ?? string.Empty;
             options.Transactions = section.GetSection("Endpoints").GetValue<string>("Transactions") ?? string.Empty;
         });
        services.AddOptions<RabbitMqOptions>()
         .Configure<IConfiguration>((options, configuration) =>
         {
             var section = configuration.GetSection("RabbitMqOptions");
             options.Host = section.GetValue<string>("Host") ?? string.Empty;
             options.Name = section.GetValue<string>("Name") ?? string.Empty;
             options.Password = section.GetValue<string>("Password") ?? string.Empty;
             options.RoleUpdaterConsumer = section.GetSection("Consumers").GetValue<string>("RoleUpdaterConsumer") ?? string.Empty;
         });
        services.AddHttpClient<ICommonHttpClient, CommonHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TransactionStoreApiConnectionStrings>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl);
            client.Timeout = new TimeSpan(0, 5, 0);
        });
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(rabbitMqOptions.Host, h =>
                {
                    h.Username(rabbitMqOptions.Name);
                    h.Password(rabbitMqOptions.Password);
                });

                cfg.ReceiveEndpoint(rabbitMqOptions.RoleUpdaterConsumer, e =>
                {
                    e.Consumer<RoleUpdaterConsumer>(context);
                });
            });
        });
    }
}

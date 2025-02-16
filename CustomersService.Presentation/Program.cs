using CustomersService.Application.Configuration;
using CustomersService.Application.Integrations;
using CustomersService.Core;
using CustomersService.Persistence.Configuration;
using CustomersService.Presentation.Configuration;
using Microsoft.Extensions.Options;

namespace CustomersService.Presentation;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
       .AddCommandLine(args)
       .AddEnvironmentVariables()
       .Build();

        builder.Services.AddSwaggerGen();
        builder.Host.ConfigureCustomLogging();
        var configuration = builder.Configuration;

        builder.Services.ConfigureMappers();
        builder.Services.AddPersistenceServices(configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddPresentationServices();
        
        builder.Services.AddHttpClient<CommonHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TransactionStoreAPIConnectionStrings>>();
            client.BaseAddress = new Uri(options.Value.BaseUrl);
            client.Timeout = new TimeSpan(0, 5, 0);
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}

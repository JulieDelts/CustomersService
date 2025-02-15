using CustomersService.Application.Configuration;
using CustomersService.Core;
using CustomersService.Persistence.Configuration;
using CustomersService.Presentation.Configuration;

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

        builder.Services.AddOptions<TransactionStoreAPIConnectionStrings>()
            .Bind(builder.Configuration.GetSection("TransactionStoreAPIConnectionStrings"));

        builder.Services.ConfigureMappers();
        builder.Services.AddPersistenceServices(configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddPresentationServices();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

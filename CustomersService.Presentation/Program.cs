using CustomersService.Application.Configuration;
using CustomersService.Persistence.Configuration;
using CustomersService.Presentation.Configuration;

namespace CustomersService.Presentation;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddCommandLine(args)
       .AddEnvironmentVariables()
       .Build();

        builder.Host.ConfigureCustomLogging();
        var configuration = builder.Configuration;

        builder.Services.ConfigureMappers();
        builder.Services.AddPersistenceServices(configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddPresentationServices();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}

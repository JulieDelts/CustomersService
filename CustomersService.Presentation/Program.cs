using CustomersService.Persistence.Configuration;
using CustomersService.Presentation.Configuration;

namespace CustomersService.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Configuration
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
           .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
           .AddCommandLine(args)
           .AddEnvironmentVariables()
           .Build();

            var configuration = builder.Configuration;

            builder.Services.AddPersistenceServices(configuration);

            builder.Services.AddPresentationServices();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

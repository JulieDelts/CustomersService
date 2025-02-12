using Serilog;

namespace CustomersService.Presentation.Configuration;
internal static class LoggingConfiguration
{
    public static void ConfigureCustomLogging(this IHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
        });
    }
}

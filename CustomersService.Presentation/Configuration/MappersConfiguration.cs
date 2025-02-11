using CustomersService.Application.Mappings;
using CustomersService.Presentation.Mappings;

namespace CustomersService.Presentation.Configuration;
public static class MappersConfiguration
{
    public static void ConfigureMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CustomerApplicationMapperProfile).Assembly);
        services.AddAutoMapper(typeof(AccountPresentationMapperProfile).Assembly);
    }
}

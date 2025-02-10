using CustomersService.Presentation.Mappings;

namespace CustomersService.Presentation.Configuration
{
    public static class ServicesConfiguration
    {
        public static void AddPresentationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CustomerPresentationMapperProfile).Assembly);
        }
    }
}

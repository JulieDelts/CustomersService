using CustomersService.Persistence.Interfaces;
using CustomersService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomersService.Persistence.Configuration;

public static class ServicesConfiguration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICustomerUnitOfWork, CustomerUnitOfWork>();
        services.AddDbContext<CustomerServiceDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CustomerServiceDefaultConnection")));
    }
}

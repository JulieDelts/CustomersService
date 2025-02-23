using CustomersService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CustomersService.Application.Integrations;

public class RoleUpdaterConsumer(ICustomerService customerService,
    ILogger<RoleUpdaterConsumer> logger): IConsumer<List<Guid>>
{
    public async Task Consume(ConsumeContext<List<Guid>> context)
    {
        logger.LogInformation("Received guids for role update from RabbitMQ {ids}", string.Join(", ", context.Message));
        await customerService.BatchUpdateRoleAsync(context.Message);
    }
}

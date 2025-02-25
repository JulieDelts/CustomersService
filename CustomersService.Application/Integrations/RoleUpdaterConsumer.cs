using CustomersService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using MYPBackendMicroserviceIntegrations.Messages;

namespace CustomersService.Application.Integrations;

public class RoleUpdaterConsumer(ICustomerService customerService,
    ILogger<RoleUpdaterConsumer> logger): IConsumer<CustomerRoleUpdateIdsMessage>
{
    public async Task Consume(ConsumeContext<CustomerRoleUpdateIdsMessage> context)
    {
        logger.LogInformation("Received guids for role update from RabbitMQ {ids}", string.Join(", ", context.Message.VipCustomerIds));
        await customerService.BatchUpdateRoleAsync(context.Message.VipCustomerIds);
    }
}

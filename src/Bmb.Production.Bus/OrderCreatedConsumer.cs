using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Production.Bus;

public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger,  IKitchenQueueGateway kitchenQueueGateway)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        logger.LogInformation("Message processed: {Message}", context.Message);
        await kitchenQueueGateway.SaveAsync(context.Message.ToDto());
    }
}
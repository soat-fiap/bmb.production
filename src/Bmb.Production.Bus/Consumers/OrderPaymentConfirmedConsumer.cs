using Bmb.Domain.Core.Events.Notifications;
using Bmb.Production.Application.UseCases;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Production.Bus.Consumers;

public class OrderPaymentConfirmedConsumer(
    ILogger<OrderPaymentConfirmedConsumer> logger,
    IReceiveOrderUseCase receiveOrderUseCase)
    : IConsumer<OrderPaymentConfirmed>
{
    public async Task Consume(ConsumeContext<OrderPaymentConfirmed> context)
    {
        try
        {
            var message = context.GetValidNotificationMessage();
            logger.LogInformation("Processing Payment confirmation for {OrderId}", context.Message.OrderId);

            await receiveOrderUseCase.ExecuteAsync(message.OrderId, context.CancellationToken);

            logger.LogInformation(
                "Successfully processed Payment confirmation for {OrderId}", context.Message.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Payment confirmation message: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}
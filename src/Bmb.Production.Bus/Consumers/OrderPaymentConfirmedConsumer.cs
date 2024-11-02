using Bmb.Domain.Core.Events.Notifications;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Model;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Production.Bus.Consumers;

public class OrderPaymentConfirmedConsumer(
    ILogger<OrderPaymentConfirmedConsumer> logger,
    IEnqueueOrderUseCase enqueueOrderUseCase)
    : IConsumer<OrderPaymentConfirmed>
{
    public async Task Consume(ConsumeContext<OrderPaymentConfirmed> context)
    {
        var message = context.GetValidNotificationMessage();
        logger.LogInformation("Processing Payment confirmation for {OrderId}", context.Message.OrderId);

        try
        {
            await ProcessPaymentConfirmationAsync(message.OrderId, context.CancellationToken);
            logger.LogInformation("Successfully processed Payment confirmation for {OrderId}",
                context.Message.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Payment confirmation message: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    private async Task ProcessPaymentConfirmationAsync(Guid orderId, CancellationToken cancellationToken)
    {
        await enqueueOrderUseCase.ExecuteAsync(orderId, cancellationToken);
    }
}
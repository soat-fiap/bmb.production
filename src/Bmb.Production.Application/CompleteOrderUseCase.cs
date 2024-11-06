using Bmb.Domain.Core.Events;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using OrderStatusChanged = Bmb.Domain.Core.Events.Notifications.OrderStatusChanged;

namespace Bmb.Production.Application;

public class CompleteOrderUseCase(
    IKitchenOrderRepository kitchenOrderRepository,
    IDispatcher dispatcher)
    : OrderUseCaseBase(kitchenOrderRepository), ICompleteOrderUseCase
{
    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);
        if (order is null)
        {
            return;
        }

        await Repository.DeleteOrderAsync(order.OrderId, cancellationToken);
        await Notify(order.OrderId, cancellationToken);
    }

    private Task Notify(Guid orderId, CancellationToken cancellationToken)
    {
        return dispatcher.PublishAsync(new OrderStatusChanged(orderId, OrderStatus.Completed),
            cancellationToken);
    }
}
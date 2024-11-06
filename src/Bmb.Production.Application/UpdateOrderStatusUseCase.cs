using Bmb.Domain.Core.Events;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model;
using OrderStatusChanged = Bmb.Domain.Core.Events.Notifications.OrderStatusChanged;

namespace Bmb.Production.Application;

public class UpdateOrderStatusUseCase(IKitchenOrderRepository repository, IDispatcher dispatcher)
    : OrderUseCaseBase(repository), IUpdateOrderStatusUseCase
{
    public async Task ExecuteAsync(Guid orderId, KitchenOrderStatus status,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(orderId, cancellationToken);
        if (order is null)
        {
            return;
        }

        await Repository.UpdateStatusAsync(order.OrderId, status, cancellationToken);
        await Notify(order.OrderId, status, cancellationToken);
    }

    private Task Notify(Guid orderId, KitchenOrderStatus status, CancellationToken cancellationToken)
    {
        if (ShouldNotify(status))
        {
            return dispatcher.PublishAsync(new OrderStatusChanged(orderId, (OrderStatus)status),
                cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static bool ShouldNotify(KitchenOrderStatus status)
    {
        return status > KitchenOrderStatus.Queued;
    }
}
using Bmb.Domain.Core.Events.Integration;
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Bus;

internal static class Presenter
{
    internal static KitchenOrderDto ToDto(this OrderCreated order)
    {
        return new KitchenOrderDto(order.Id, order.OrderTrackingCode, order.Items.ToDto(), KitchenOrderStatus.Processing);
    }

    private static List<OrderItemDto> ToDto(this List<OrderCreated.OrderItemReplicaDto>? items)
    {
        var dtoItems = new List<OrderItemDto>();
        if (items != null)
            dtoItems.AddRange(items.Select(x =>
                new OrderItemDto(x.ProductName, x.Quantity)));
        return dtoItems;
    }
}
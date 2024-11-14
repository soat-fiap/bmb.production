using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application.Dtos;

public static class Mapper
{
    public static KitchenQueueItem MapToKitchenQueueItem(this KitchenOrderDto order)
    {
        return new KitchenQueueItem(order.OrderId, order.OrderTrackingCode);
    }
}
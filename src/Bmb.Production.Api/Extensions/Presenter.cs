using Bmb.Production.Api.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Api.Extensions;

public static class Presenter
{
    public static NextOrderResponse ToNextOrderResponse(this KitchenOrderDto kitchenOrderDto)
    {
        return new NextOrderResponse(kitchenOrderDto.OrderId, kitchenOrderDto.OrderTrackingCode,
            kitchenOrderDto.Items.Select(x => new OrderItemResponse(x.ProductName, x.Quantity)).ToList());
    }
}
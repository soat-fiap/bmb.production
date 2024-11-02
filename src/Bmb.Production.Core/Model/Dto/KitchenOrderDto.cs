namespace Bmb.Production.Core.Model.Dto;

public class KitchenOrderDto
{
    public Guid OrderId { get; init; }
    public string OrderTrackingCode { get; init; }
    public IReadOnlyCollection<OrderItemDto> Items { get; init; }
    public KitchenOrderStatus? Status { get; set; }

    public KitchenOrderDto(Guid orderId, string orderTrackingCode, IReadOnlyCollection<OrderItemDto> items, KitchenOrderStatus? status = null)
    {
        OrderId = orderId;
        OrderTrackingCode = orderTrackingCode;
        Items = items;
        Status = status;
    }
}

public record OrderItemDto(string ProductName, int Quantity);
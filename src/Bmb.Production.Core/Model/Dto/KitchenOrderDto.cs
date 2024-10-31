namespace Bmb.Production.Core.Model.Dto;

public record KitchenOrderDto(Guid OrderId, string OrderTrackingCode, IReadOnlyCollection<OrderItemDto> Items, KitchenOrderStatus? Status = null);

public record OrderItemDto(string ProductName, int Quantity);
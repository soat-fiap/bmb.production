namespace Bmb.Production.Api.Model;

/// <summary>
/// 
/// </summary>/// <summary>
/// Represents the response for the next order.
/// </summary>
public class NextOrderResponse
{
    /// <summary>
    /// Gets the unique identifier for the order.
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Gets the tracking code for the order.
    /// </summary>
    public string OrderTrackingCode { get; init; }

    /// <summary>
    /// Gets the collection of items in the order.
    /// </summary>
    public IReadOnlyCollection<OrderItemResponse> Items { get; init; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NextOrderResponse"/> class.
    /// </summary>
    /// <param name="orderId">The unique identifier for the order.</param>
    /// <param name="orderTrackingCode">The tracking code for the order.</param>
    /// <param name="items">The collection of items in the order.</param>
    public NextOrderResponse(Guid orderId, string orderTrackingCode, IReadOnlyCollection<OrderItemResponse> items)
    {
        OrderId = orderId;
        OrderTrackingCode = orderTrackingCode;
        Items = items;
    }
}

/// <summary>
/// Represents an item in the order.
/// </summary>
/// <param name="ProductName">The name of the product.</param>
/// <param name="Quantity">The quantity of the product.</param>
public record OrderItemResponse(string ProductName, int Quantity);



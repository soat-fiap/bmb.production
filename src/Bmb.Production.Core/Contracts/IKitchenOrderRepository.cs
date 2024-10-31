using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Core.Contracts;

public interface IKitchenOrderRepository
{
    /// <summary>
    /// Save Order.
    /// </summary>
    /// <param name="order">Order to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve all orders.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of all orders.</returns>
    Task<IReadOnlyCollection<KitchenOrderDto>> GetAllAsync(CancellationToken cancellationToken = default);

    
    /// <summary>
    /// Retrieve an order by ID.
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Order if found, otherwise null.</returns>
    Task<KitchenOrderDto?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueue an order.
    /// </summary>
    /// <param name="order">Order to enqueue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task EnqueueOrderAsync(KitchenOrderDto order,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieve the next order from the queue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Next order if found, otherwise null.</returns>
    Task<KitchenOrderDto?> GetNextOrderAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the status of an order.
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <param name="status">New status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>

    Task UpdateStatusAsync(Guid orderId, KitchenOrderStatus status,
        CancellationToken cancellationToken = default);
    
    
    /// <summary>
    /// Delete an order.
    /// </summary>
    /// <param name="orderId">Order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);


}
using Bmb.Production.Core.Model;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Core.Contracts;

public interface IKitchenOrderRepository
{
    Task SaveAsync(KitchenOrderDto order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all orders from the queue
    /// </summary>
    /// <param name="queue">Order queue</param>
    /// <param name="cancellationToken">Cancellation token. </param>
    /// <returns></returns>
    Task<IReadOnlyCollection<string>> GetAllAsync(KitchenQueue queue, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get order by ID
    /// </summary>
    /// <param name="orderId">Order id</param>
    /// <param name="cancellationToken">Cancellation token. </param>
    /// <returns></returns>
    Task<KitchenOrderDto?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);
}
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application;

public abstract class OrderUseCaseBase
{
    protected readonly IKitchenOrderRepository Repository;

    protected OrderUseCaseBase(IKitchenOrderRepository repository)
    {
        Repository = repository;
    }

    protected Task<KitchenOrderDto?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return Repository.GetAsync(orderId, cancellationToken);
    }
}
using Bmb.Production.Application.UseCases;
using Bmb.Production.Core.Contracts;
using Bmb.Production.Core.Model.Dto;

namespace Bmb.Production.Application;

public class GetNextOrderUseCase(IKitchenOrderRepository repository) : IGetNextOrderUseCase
{
    public Task<KitchenOrderDto?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetNextOrderAsync(cancellationToken);
    }
}

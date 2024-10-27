using Bmb.Production.Application.Dtos;

namespace Bmb.Production.Application.UseCases;

public interface IGetKitchenLineUseCase
{
    Task<KitchenQueueResponse> ExecuteAsync(CancellationToken cancellationToken = default);
}
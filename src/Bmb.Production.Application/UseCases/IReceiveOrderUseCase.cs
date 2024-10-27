namespace Bmb.Production.Application.UseCases;

public interface IReceiveOrderUseCase
{
    Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}
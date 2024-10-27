using Bmb.Production.Application.UseCases;

namespace Bmb.Production.Application;

public class ReceiveOrderUseCase : IReceiveOrderUseCase
{
    public Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
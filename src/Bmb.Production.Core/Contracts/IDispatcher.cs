using Bmb.Domain.Core.Events.Notifications;

namespace Bmb.Production.Core.Contracts;

public interface IDispatcher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IBmbNotification;
}
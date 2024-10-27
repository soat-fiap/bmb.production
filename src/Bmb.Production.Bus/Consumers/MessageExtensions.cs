using Bmb.Domain.Core.Events.Integration;
using Bmb.Domain.Core.Events.Notifications;
using MassTransit;

namespace Bmb.Production.Bus.Consumers;

internal static class MessageExtensions
{
    internal static T GetValidIntegrationMessage<T>(this ConsumeContext<T> context) where T : class, IBmbIntegrationEvent
    {
        var message = context.Message;
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), $"{typeof(T).Name} message cannot be null");
        }

        return message;
    } 
    
    internal static T GetValidNotificationMessage<T>(this ConsumeContext<T> context) where T : class, IBmbNotification
    {
        var message = context.Message;
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), $"{typeof(T).Name} message cannot be null");
        }

        return message;
    } 
}
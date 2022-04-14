using MediatR;

namespace RabbitMqEventBus.HandlerManager;

public interface IHandlerManagerConfigurator
{
    /// <summary>
    /// Use this method to bind notification with message type
    /// </summary>
    /// <param name="messageType">Message type to execute generic notification</param>
    /// <typeparam name="TNotification">Notification type (must implement MediatR.INotification interface)</typeparam>
    public void AddNotificationForMessageType<TNotification>(string messageType) where TNotification : INotification;
}
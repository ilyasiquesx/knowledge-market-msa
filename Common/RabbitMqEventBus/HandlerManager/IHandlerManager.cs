namespace RabbitMqEventBus.HandlerManager;

public interface IHandlerManager
{
    public Task ResolveHandlerForMessage(string messageType, string message);
}
namespace RabbitMqEventBus.MessagePublisher;

public interface IMessagePublisher
{
    Task PublishAsync(string messageType, object message);
}
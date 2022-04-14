namespace RabbitMqEventBus.MessageHandler;

public interface IMessageHandler
{
    void StartConsume(IMessageCallback callback);
}
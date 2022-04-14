namespace RabbitMqEventBus.MessageHandler;

public interface IMessageCallback
{
    public Task Invoke(string messageType, string messageBody);
}
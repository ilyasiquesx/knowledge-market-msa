using System;
using System.Threading.Tasks;
using RabbitMqEventBus.MessageHandler;
using RabbitMqEventBus.MessagePublisher;

namespace Forum.IntegrationTests;

public class MockHandlerPublisher : IMessageHandler, IMessagePublisher
{
    public void StartConsume(IMessageCallback callback)
    {
        Console.WriteLine("Started consume");
    }

    public Task PublishAsync(string messageType, object message)
    {
        Console.WriteLine($"{messageType} - {message}");
        return Task.CompletedTask;
    }
}
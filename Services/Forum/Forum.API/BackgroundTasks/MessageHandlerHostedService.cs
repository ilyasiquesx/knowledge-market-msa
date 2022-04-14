using RabbitMqEventBus.HandlerManager;
using RabbitMqEventBus.MessageHandler;

namespace Forum.API.BackgroundTasks;

public class MessageHandlerHostedService : BackgroundService, IMessageCallback
{
    private readonly IMessageHandler _messageHandler;
    private readonly IHandlerManager _handlerManager;

    public MessageHandlerHostedService(IMessageHandler handler,
        IHandlerManager handlerManager)
    {
        _messageHandler = handler;
        _handlerManager = handlerManager;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageHandler.StartConsume(this);
        return Task.CompletedTask;
    }

    public async Task Invoke(string messageType, string messageBody)
    {
        await _handlerManager.ResolveHandlerForMessage(messageType, messageBody);
    }
}
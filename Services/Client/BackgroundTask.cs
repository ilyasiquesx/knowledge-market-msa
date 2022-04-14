using RabbitMqEventBus.HandlerManager;
using RabbitMqEventBus.MessageHandler;

namespace Client;

public class BackgroundTask : BackgroundService, IMessageCallback
{
    private readonly IMessageHandler _messageHandler;
    private readonly IHandlerManager _handlerManager;

    public BackgroundTask(IMessageHandler messageHandler,
        IHandlerManager handlerManager)
    {
        _messageHandler = messageHandler;
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
using RabbitMqEventBus.HandlerManager;
using RabbitMqEventBus.MessageHandler;

namespace Notifications.API.BackgroundTasks;

public class IntegrationMessageHandlerHostedService : BackgroundService, IMessageCallback
{
    private readonly IMessageHandler _handler;
    private readonly IHandlerManager _handlerManager;

    public IntegrationMessageHandlerHostedService(IMessageHandler handler, IHandlerManager handlerManager)
    {
        _handler = handler;
        _handlerManager = handlerManager;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _handler.StartConsume(this);
        return Task.CompletedTask;
    }

    public async Task Invoke(string messageType, string messageBody)
    {
        await _handlerManager.ResolveHandlerForMessage(messageType, messageBody);
    }
}
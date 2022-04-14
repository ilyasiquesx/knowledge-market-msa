using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RabbitMqEventBus.HandlerManager;

internal sealed class HandlerManager : IHandlerManager, IHandlerManagerConfigurator
{
    private readonly IDictionary<string, Type> _registeredHandlers = new Dictionary<string, Type>();
    private readonly ILogger<HandlerManager> _logger;
    private readonly IServiceProvider _serviceProvider;

    public HandlerManager(ILogger<HandlerManager> logger,
        Action<IHandlerManagerConfigurator> configureHandlers, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        configureHandlers(this);
    }

    public async Task ResolveHandlerForMessage(string messageType, string messageBody)
    {
        var notificationTypeExists = _registeredHandlers.TryGetValue(messageType, out var notificationType);
        if (notificationTypeExists &&
            JsonConvert.DeserializeObject(messageBody, notificationType) is INotification notification)
        {
            _logger.LogInformation("Found notification for message type: {MessageType}", messageType);
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            if (mediator != null)
            {
                await mediator.Publish(notification);
                return;
            }
        }

        _logger.LogInformation("Notification for message type: {MessageType} wasn't found. Ignoring", messageType);
    }

    public void AddNotificationForMessageType<T>(string messageType) where T : INotification
    {
        _registeredHandlers.TryAdd(messageType, typeof(T));
    }
}
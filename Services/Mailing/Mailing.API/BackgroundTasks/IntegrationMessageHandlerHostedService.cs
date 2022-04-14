using Mailing.API.Data;
using Mailing.API.Data.Entities;
using RabbitMqEventBus.MessageHandler;

namespace Mailing.API.BackgroundTasks;

public class IntegrationMessageHandlerHostedService : BackgroundService, IMessageCallback
{
    private readonly IMessageHandler _handler;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationMessageHandlerHostedService> _logger;

    public IntegrationMessageHandlerHostedService(IMessageHandler handler, IServiceProvider serviceProvider, 
        ILogger<IntegrationMessageHandlerHostedService> logger)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _handler.StartConsume(this);
        return Task.CompletedTask;
    }

    public async Task Invoke(string messageType, string messageBody)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MailingContext>();
        context.Add(new InboxMessage
        {
            Body = messageBody,
            Type = messageType,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        
        _logger.LogInformation("{Message} {MessageType} {MessageBody}", "Message is saved for processing later", messageType, messageBody);
    }
}
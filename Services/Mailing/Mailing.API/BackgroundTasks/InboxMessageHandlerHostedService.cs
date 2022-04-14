using Mailing.API.Data;
using RabbitMqEventBus.HandlerManager;

namespace Mailing.API.BackgroundTasks;

public class InboxMessageHandlerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHandlerManager _handlerManager;
    private readonly ILogger<InboxMessageHandlerHostedService> _logger;
    private readonly int _emptyInboxDelay;
    private const string LogTemplate = "{Message} {MessageType} {MessageBody}";

    public InboxMessageHandlerHostedService(IHandlerManager handlerManager,
        IServiceProvider serviceProvider, ILogger<InboxMessageHandlerHostedService> logger, IConfiguration configuration)
    {
        _handlerManager = handlerManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _emptyInboxDelay = configuration.GetValue<int>("EmptyInboxDelay");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ProcessInbox(stoppingToken);
    }

    private async Task ProcessInbox(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MailingContext>();
            var message = await context.GetOldestMessage();
            if (message != null)
            {
                _logger.LogInformation(LogTemplate, "Inbox message is got for processing", message.Type, message.Body);
                try
                {
                    await _handlerManager.ResolveHandlerForMessage(message.Type, message.Body);
                    _logger.LogInformation(LogTemplate, "Inbox message was processed correctly", message.Type, message.Body);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, LogTemplate, "Processing inbox message error", message.Type, message.Body);
                }
                finally
                {
                    context.InboxMessages.Remove(message);
                    await context.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation(LogTemplate, "Inbox message has been removed", message.Type, message.Body);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(_emptyInboxDelay), stoppingToken);
        }
    }
}
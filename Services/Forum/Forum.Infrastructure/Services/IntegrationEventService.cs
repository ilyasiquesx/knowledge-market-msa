using Forum.Core.Services;
using RabbitMqEventBus.MessagePublisher;

namespace Forum.Infrastructure.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IMessagePublisher _publisher;

    public IntegrationEventService(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Publish(string eventName, object details)
    {
        await _publisher.PublishAsync(eventName, details);
    }
}
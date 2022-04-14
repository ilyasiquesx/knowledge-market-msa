namespace Forum.Core.Services;

public interface IIntegrationEventService
{
    Task Publish(string eventName, object details);
}
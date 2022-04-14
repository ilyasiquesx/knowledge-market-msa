using MediatR;

namespace Client;

public class UserCreatedNotification : INotification
{
    
}

public class NotHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Started...");
        Thread.Sleep(5000);
        Console.WriteLine("Cancelled");
        return Task.CompletedTask;
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;
using Notifications.API.Dto;

namespace Notifications.API.MediatrNotifications.Users;

public class UserCreatedNotification : INotification
{
    public string UserId { get; set; }
    public string Username { get; set; }
}

public class UserCreatedHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly ApplicationContext _context;

    public UserCreatedHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.UserId, cancellationToken);
        if (user != null)
            return;

        _context.Users.Add(new User
        {
            Username = notification.Username,
            Id = notification.UserId
        });
        
        _context.Notifications.Add(new Notification
        {
            Content = "Thank you for choosing our service! We hope you will find answers to your questions!",
            CreatedAt = DateTime.UtcNow,
            UserId = notification.UserId
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
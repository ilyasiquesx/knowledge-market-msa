using Forum.Core.Services;
using MediatR;

namespace Forum.Core.Entities.Users.Notifications;

public class CreateUserNotification : INotification
{
    public string UserId { get; set; }
    public string Username { get; set; }
}

public class CreateUserNotificationHandler : INotificationHandler<CreateUserNotification>
{
    private readonly IDomainContext _context;

    public CreateUserNotificationHandler(IDomainContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateUserNotification notification, CancellationToken cancellationToken)
    {
        if (_context.Users.FirstOrDefault(u => u.Id == notification.UserId) != null)
            return;

        _context.Users.Add(new User
        {
            Id = notification.UserId,
            Username = notification.Username
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
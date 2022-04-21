using Mailing.API.Data;
using Mailing.API.Data.Entities;
using Mailing.API.MessageBuilding;
using Mailing.API.Smtp;
using MediatR;

namespace Mailing.API.Notifications.Users;

public class UserCreatedNotification : INotification
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsSubscribedForMailing { get; set; }
}

public class UserCreatedHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly MailingContext _context;
    private readonly ISmtpSender _smtpSender;
    private readonly IMessageBuilder _messageBuilder;

    public UserCreatedHandler(MailingContext context,
        ISmtpSender sender,
        IMessageBuilderProvider messageBuilderProvider)
    {
        _context = context;
        _smtpSender = sender;
        _messageBuilder = messageBuilderProvider.GetByType("UserCreated");
    }

    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = notification.UserId,
            Username = notification.Username,
            Email = notification.Email,
            IsSubscribedForMailing = notification.IsSubscribedForMailing
        };
        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);
        
        if (user.IsSubscribedForMailing)
        {
            var customParams = new Dictionary<string, object>()
            {
                { "username", user.Username }
            };

            var message = _messageBuilder.BuildMessage(customParams);
            _smtpSender.Send(user.Id, user.Username, user.Email, message.Subject, message.Content);
        }
    }
}
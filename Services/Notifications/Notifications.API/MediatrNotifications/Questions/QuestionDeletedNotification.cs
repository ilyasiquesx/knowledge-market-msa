using MediatR;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;

namespace Notifications.API.MediatrNotifications.Questions;

public class QuestionDeletedNotification : INotification
{
    public long Id { get; set; }
}

public class QuestionDeletedHandler : INotificationHandler<QuestionDeletedNotification>
{
    private readonly ApplicationContext _context;

    public QuestionDeletedHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task Handle(QuestionDeletedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == notification.Id, cancellationToken);
        if (question == null)
            return;

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
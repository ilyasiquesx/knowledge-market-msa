using MediatR;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;

namespace Notifications.API.MediatrNotifications.Questions;

public class QuestionUpdatedNotification : INotification
{
    public long Id { get; set; }
    public string Title { get; set; }
}

public class QuestionUpdatedHandler : INotificationHandler<QuestionUpdatedNotification>
{
    private readonly ApplicationContext _context;

    public QuestionUpdatedHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task Handle(QuestionUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == notification.Id, cancellationToken);
        if (question == null)
            return;

        question.Title = notification.Title;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
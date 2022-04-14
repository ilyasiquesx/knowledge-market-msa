using Mailing.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Notifications.Questions;

public class QuestionUpdatedNotification : INotification
{
    public long Id { get; set; }
    public string Title { get; set; }
}

public class QuestionUpdatedHandler : INotificationHandler<QuestionUpdatedNotification>
{
    private readonly MailingContext _context;

    public QuestionUpdatedHandler(MailingContext context)
    {
        _context = context;
    }

    public async Task Handle(QuestionUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == notification.Id, cancellationToken);
        if (question == null)
            return;

        question.Title = notification.Title;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
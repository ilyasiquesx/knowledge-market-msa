using Mailing.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Notifications.Questions;

public class QuestionDeletedNotification : INotification
{
    public long Id { get; set; }
}

public class QuestionDeletedHandler : INotificationHandler<QuestionDeletedNotification>
{
    private readonly MailingContext _context;

    public QuestionDeletedHandler(MailingContext context)
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
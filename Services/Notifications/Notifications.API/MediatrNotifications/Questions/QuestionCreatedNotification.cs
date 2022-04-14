using MediatR;
using Microsoft.EntityFrameworkCore;
using Notifications.API.Data;

namespace Notifications.API.MediatrNotifications.Questions;

public class QuestionCreatedNotification : INotification
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string AuthorId { get; set; }
}

public class QuestionCreatedHandler : INotificationHandler<QuestionCreatedNotification>
{
    private readonly ApplicationContext _context;

    public QuestionCreatedHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task Handle(QuestionCreatedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == notification.Id, cancellationToken);
        if (question != null)
            return;

        _context.Questions.Add(new Question
        {
            Id = notification.Id,
            Title = notification.Title,
            AuthorId = notification.AuthorId
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
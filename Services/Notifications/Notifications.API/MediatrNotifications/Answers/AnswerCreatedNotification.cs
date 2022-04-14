using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notifications.API.Data;

namespace Notifications.API.MediatrNotifications.Answers;

public class AnswerCreatedNotification : INotification
{
    public long QuestionId { get; set; }
    public string CommentAuthorId { get; set; }
}


public class AnswerCreatedHandler : INotificationHandler<AnswerCreatedNotification>
{
    private readonly ApplicationContext _context;

    public AnswerCreatedHandler(ApplicationContext context)
    {
        _context = context;
    }

    public async Task Handle(AnswerCreatedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == notification.QuestionId, cancellationToken);

        if (question?.AuthorId == null)
            return;

        var commentAuthor = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == notification.CommentAuthorId, cancellationToken);

        if (commentAuthor == null || commentAuthor.Id == question.AuthorId)
            return;

        var message = $"{commentAuthor.Username} has answered your question: {question.Title}";
        var questionRelativeLink = $"/questions/{question.Id}";

        var serializedNotificationContent = JsonConvert.SerializeObject(new
        {
            message,
            link = questionRelativeLink
        });

        var domainNotification = new Notification
        {
            UserId = question.AuthorId,
            Content = serializedNotificationContent,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(domainNotification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
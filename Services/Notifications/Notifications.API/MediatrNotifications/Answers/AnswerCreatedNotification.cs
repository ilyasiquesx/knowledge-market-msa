using MediatR;
using Microsoft.EntityFrameworkCore;
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
    private readonly string _clientUrl;
    private readonly string _questionLinkTemplate;

    public AnswerCreatedHandler(ApplicationContext context, IConfiguration configuration)
    {
        _context = context;
        _clientUrl = configuration.GetValue<string>("ClientUrl");
        _questionLinkTemplate = configuration.GetValue<string>("QuestionLinkTemplate");
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

        var questionLink = string.Format(_questionLinkTemplate, _clientUrl, question.Id);

        var message =
            @$"{commentAuthor.Username} has answered your question: <a href=""{questionLink}"">{question.Title}</a>";

        var domainNotification = new Notification
        {
            UserId = question.AuthorId,
            Content = message,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(domainNotification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
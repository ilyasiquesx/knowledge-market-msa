using Mailing.API.Data;
using Mailing.API.Data.Entities;
using MediatR;

namespace Mailing.API.Notifications.Questions;

public class QuestionCreatedNotification : INotification
{
    public string Title { get; set; }
    public long Id { get; set; }
    public string AuthorId { get; set; }
}

public class QuestionCreatedHandler : INotificationHandler<QuestionCreatedNotification>
{
    private readonly MailingContext _context;

    public QuestionCreatedHandler(MailingContext context)
    {
        _context = context;
    }

    public async Task Handle(QuestionCreatedNotification notification, CancellationToken cancellationToken)
    {
        _context.Questions.Add(new Question
        {
            AuthorId = notification.AuthorId,
            Id = notification.Id,
            Title = notification.Title
        });

        await _context.SaveChangesAsync(cancellationToken);
    }
}
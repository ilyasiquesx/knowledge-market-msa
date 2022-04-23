using Mailing.API.Data;
using Mailing.API.MessageBuilding;
using Mailing.API.Smtp;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Notifications.Answers;

public class AnswerCreatedNotification : INotification
{
    public long QuestionId { get; set; }
    public string CommentAuthorId { get; set; }
}

public class AnswerCreatedHandler : INotificationHandler<AnswerCreatedNotification>
{
    private readonly MailingContext _context;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ISmtpSender _smtpSender;
    private readonly string _questionLinkTemplate;
    private readonly string _clientUrl;

    public AnswerCreatedHandler(MailingContext context,
        IMessageBuilderProvider builderProvider,
        ISmtpSender smtpSender, IConfiguration configuration)
    {
        _context = context;
        _messageBuilder = builderProvider.GetByType("AnswerCreated");
        _smtpSender = smtpSender;
        _questionLinkTemplate = configuration.GetValue<string>("QuestionLinkTemplate");
        _clientUrl = configuration.GetValue<string>("ClientUrl");
    }

    public async Task Handle(AnswerCreatedNotification notification, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == notification.QuestionId, cancellationToken);

        var commentAuthor = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == notification.CommentAuthorId, cancellationToken);

        if (question?.Author?.Username == null
            || commentAuthor?.Username == null
            || question.AuthorId == commentAuthor.Id)
        {
            return;
        }

        if (question.Author.IsSubscribedForMailing)
        {
            var customParams = new Dictionary<string, object>();
            customParams.TryAdd("question", question);
            customParams.TryAdd("commentAuthor", commentAuthor);
            customParams.TryAdd("questionLink", string.Format(_questionLinkTemplate, _clientUrl, question.Id));
            var messageDetails = _messageBuilder.BuildMessage(customParams);
            _smtpSender.Send(question.Author.Id, question.Author.Username, question.Author.Email, messageDetails.Subject,
                messageDetails.Content);
        }
    }
}
using Forum.Core.Entities.Answers;
using Forum.Core.Entities.Questions;
using Forum.Core.Services;
using MediatR;

namespace Forum.Core.Entities.Users.Notifications;

public class SampleUserNotification : INotification
{
    public string Id { get; set; }
    public string Username { get; set; }
}

public class SampleUserNotificationHandler : INotificationHandler<SampleUserNotification>
{
    private readonly IDomainContext _domainContext;

    public SampleUserNotificationHandler(IDomainContext domainContext)
    {
        _domainContext = domainContext;
    }

    public async Task Handle(SampleUserNotification notification, CancellationToken cancellationToken)
    { 
        var user = new User
        {
            Id = notification.Id,
            Username = notification.Username
        };

        var question = new Question
        {
            Title = "Sample question",
            Content = "This is a sample question!",
            Author = user
        };

        var answer = new Answer
        {
            Content = "This is a sample answer...",
            Author = user
        };
        
        question.Answers = new List<Answer>
        {
            answer
        };

        user.Questions = new List<Question>
        {
            question
        };

        _domainContext.Users.Add(user);
        await _domainContext.SaveChangesAsync(cancellationToken);
    }
}
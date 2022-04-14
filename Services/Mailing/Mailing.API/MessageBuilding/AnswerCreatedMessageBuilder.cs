using System.Text;
using Mailing.API.Data.Entities;

namespace Mailing.API.MessageBuilding;

public class AnswerCreatedMessageBuilder : IMessageBuilder
{
    public MessageDetails BuildMessage(Dictionary<string, object> customParams)
    {
        if (customParams == null)
            throw new ArgumentNullException(nameof(customParams));

        customParams.TryGetValue("question", out var question);
        customParams.TryGetValue("commentAuthor", out var commentAuthor);
        customParams.TryGetValue("questionLink", out var questionLink);

        var sb = new StringBuilder();

        if (commentAuthor is User u)
        {
            sb.AppendLine($"{u.Username} has answered you.");
        }

        if (question is Question q)
        {
            sb.AppendLine($"There is a new answer in your topic: {q.Title}!");
            if (questionLink is string ql)
                sb.AppendLine($"You can see new answers using following link: {ql}");
        }
        
        return new MessageDetails
        {
            Subject = "New answers for you",
            Content = sb.ToString()
        };
    }

    public string Type => "AnswerCreated";
}
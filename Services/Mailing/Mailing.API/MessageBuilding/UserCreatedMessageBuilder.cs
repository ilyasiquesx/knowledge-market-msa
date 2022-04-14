using System.Text;

namespace Mailing.API.MessageBuilding;

public class UserCreatedMessageBuilder : IMessageBuilder
{
    public string Type => "UserCreated";

    public MessageDetails BuildMessage(Dictionary<string, object> customParams)
    {
        if (customParams == null)
            throw new ArgumentNullException(nameof(customParams));

        customParams.TryGetValue("username", out var username);
        var userName = username as string ?? "Stranger";
        var sb = new StringBuilder();
        sb.AppendLine($"Thanks for the registration, {userName}. Hope you'll enjoy this service!");
        
        return new MessageDetails
        {
            Subject = "Welcome to the Knowledge market!",
            Content = sb.ToString()
        };
    }
}
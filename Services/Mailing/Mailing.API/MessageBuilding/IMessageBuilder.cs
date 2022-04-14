namespace Mailing.API.MessageBuilding;

public class MessageDetails
{
    public string Subject { get; set; }
    public string Content { get; set; }
}

public interface IMessageBuilder
{
    public MessageDetails BuildMessage(Dictionary<string, object> customParams);
    public string Type { get; }
}
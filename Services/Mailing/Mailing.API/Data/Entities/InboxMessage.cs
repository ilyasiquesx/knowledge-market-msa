namespace Mailing.API.Data.Entities;

public class InboxMessage
{
    public long Id { get; set; }
    public string Type { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}
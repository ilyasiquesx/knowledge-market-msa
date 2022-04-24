namespace Notifications.API.Data;

public class Notification
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
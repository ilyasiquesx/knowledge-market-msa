namespace Notifications.API.Data;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}
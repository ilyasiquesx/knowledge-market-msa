namespace Notifications.API.Data;

public class Question
{
    public long Id { get; set; }
    public string Title { get; set; }
    public User Author { get; set; }
    public string AuthorId { get; set; }
}
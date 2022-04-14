namespace Mailing.API.Data.Entities;

public class Question
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
}
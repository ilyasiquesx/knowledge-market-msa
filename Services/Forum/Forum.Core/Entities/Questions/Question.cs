using Forum.Core.Entities.Answers;
using Forum.Core.Entities.Users;

namespace Forum.Core.Entities.Questions;

public class Question
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public long? BestAnswerId { get; set; }
    public Answer BestAnswer { get; set; }
    public DateTime CreatedAt { get; set; }
}
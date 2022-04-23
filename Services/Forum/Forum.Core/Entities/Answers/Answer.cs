using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Users;

namespace Forum.Core.Entities.Answers;

public class Answer
{
    public long Id { get; set; }
    public long QuestionId { get; set; }
    public Question Question { get; set; }
    public string Content { get; set; }
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
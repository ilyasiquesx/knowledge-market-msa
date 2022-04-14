using Forum.Core.Entities.Questions;

namespace Forum.Core.Entities.Users;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public ICollection<Question> Questions { get; set; }
}
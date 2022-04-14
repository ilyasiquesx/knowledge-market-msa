
namespace Forum.Core.Entities.Questions.Queries;

public class QuestionDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public AuthorDto Author { get; set; }
    public AnswerDto BestAnswer { get; set; }
    public IEnumerable<AnswerDto> Answers { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AnswerDto
{
    public long Id { get; set; }
    public string Content { get; set; }
    public AuthorDto Author { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuthorDto
{
    public string Id { get; set; }
    public string Username { get; set; }
}
namespace Forum.Core.Entities.Questions.Queries.Get;

public class QuestionDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public AuthorDto Author { get; set; }
    public AnswerDto BestAnswer { get; set; }
    public IEnumerable<AnswerDto> Answers { get; set; }
    public string CreatedAt { get; set; }

    public bool AvailableToEdit { get; set; }
}

public class AnswerDto
{
    public long Id { get; set; }
    public string Content { get; set; }
    public AuthorDto Author { get; set; }
    public string CreatedAt { get; set; }
    public bool AvailableToEdit { get; set; }
}
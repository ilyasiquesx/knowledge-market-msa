namespace Forum.Core.Entities.Questions.Queries.GetPaginated;

public class QuestionsDto
{
    public IEnumerable<QuestionDtoTiny> Questions { get; set; }
}

public class QuestionDtoTiny
{
    public long Id { get; set; }
    public string Title { get; set; }
    public AuthorDto Author { get; set; }
    public DateTime CreatedAt { get; set; }
}
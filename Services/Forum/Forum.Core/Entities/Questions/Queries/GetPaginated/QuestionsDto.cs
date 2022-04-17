namespace Forum.Core.Entities.Questions.Queries.GetPaginated;

public class QuestionsDto
{
    public int PageCount { get; set; }
    public IEnumerable<QuestionDtoTiny> Questions { get; set; }
}

public class QuestionDtoTiny
{
    public long Id { get; set; }
    public string Title { get; set; }
    public AuthorDto Author { get; set; }
    public string CreatedAt { get; set; }
}
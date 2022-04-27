namespace Forum.Core.Entities.Questions.Queries.GetPaginated;

public class Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int MaxPageSize { get; set; } = int.MaxValue;
    public int TotalCount { get; set; }
}
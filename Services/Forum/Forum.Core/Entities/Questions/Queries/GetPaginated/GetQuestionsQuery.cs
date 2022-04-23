using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Queries.GetPaginated;

public class GetQuestionsQuery : IRequest<QuestionsDto>
{
    public Pagination Pagination { get; }

    public GetQuestionsQuery(Pagination pagination)
    {
        Pagination = pagination;
    }
}

public class Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int MaxPageSize { get; set; } = int.MaxValue;
    public int TotalCount { get; set; }
}

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, QuestionsDto>
{
    private readonly IDomainContext _domainContext;

    public GetQuestionsQueryHandler(IDomainContext domainContext)
    {
        _domainContext = domainContext;
    }

    public async Task<QuestionsDto> Handle(GetQuestionsQuery request,
        CancellationToken cancellationToken)
    {
        if (request?.Pagination == null)
            return null;

        var totalQuestions = _domainContext.Questions.Count();
        request.Pagination.TotalCount = totalQuestions;
        var totalPages = ModifyRequestAndGetTotalPages(request.Pagination);

        var pagination = request.Pagination;

        var questions = await _domainContext.Questions
            .Include(q => q.BestAnswer)
            .Include(q => q.Answers)
            .ThenInclude(a => a.Author)
            .Include(q => q.Author)
            .OrderByDescending(q => q.UpdatedAt)
            .ThenByDescending(q => q.CreatedAt)
            .Skip(pagination.PageSize * (pagination.Page - 1))
            .Take(pagination.PageSize).ToListAsync(cancellationToken);

        var questionsDto = questions.Select(Builders.Questions.BuildQuestionDto);
        return new QuestionsDto
        {
            Questions = questionsDto,
            PageCount = totalPages
        };
    }

    private static int ModifyRequestAndGetTotalPages(Pagination pagination)
    {
        if (pagination == null)
            return 0;

        if (pagination.Page < 1)
            pagination.Page = 1;

        if (pagination.PageSize > pagination.MaxPageSize)
            pagination.PageSize = pagination.MaxPageSize;

        if (pagination.PageSize < 1)
            pagination.PageSize = 15;

        var maxPages = (pagination.TotalCount + pagination.PageSize - 1) / pagination.PageSize;
        if (pagination.Page > maxPages && maxPages > 0)
            pagination.Page = maxPages;

        return maxPages;
    }
}
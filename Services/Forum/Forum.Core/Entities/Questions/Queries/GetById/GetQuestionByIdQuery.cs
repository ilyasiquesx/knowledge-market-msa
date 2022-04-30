using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Queries.GetById;

public class GetQuestionByIdQuery : IRequest<OneOf<QuestionDto, NotFoundResult, InvalidDomainBehaviorResult>>
{
    public long Id { get; }

    public GetQuestionByIdQuery(long id)
    {
        Id = id;
    }
}

public class GetQuestionQueryHandler : IRequestHandler<GetQuestionByIdQuery,
    OneOf<QuestionDto, NotFoundResult, InvalidDomainBehaviorResult>>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;

    public GetQuestionQueryHandler(IDomainContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<OneOf<QuestionDto, NotFoundResult, InvalidDomainBehaviorResult>> Handle(GetQuestionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .ThenInclude(a => a.Author)
            .Include(q => q.BestAnswer)
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

        if (question == null)
            return new NotFoundResult($"There is no question with such id: {request.Id}");

        if (question.Author == null)
            return new InvalidDomainBehaviorResult("The question must have an author");

        var author = question.Author;
        var answers = question.Answers.OrderByDescending(a => a.CreatedAt).ToList();
        if (answers.Any(a => string.IsNullOrEmpty(a.AuthorId) || a.Author == null))
            return new InvalidDomainBehaviorResult($"The answer must have an author");

        var bestAnswer = question.BestAnswer;
        AnswerDto bestAnswerDto = null;
        var requestedBy = _userService.UserId;

        if (bestAnswer != null)
        {
            answers = answers.Where(a => a.Id != bestAnswer.Id).ToList();
            bestAnswerDto = DtoBuilders.Answers.BuildAnswerDto(bestAnswer, requestedBy);
        }

        var dto = new QuestionDto
        {
            Id = question.Id,
            Title = question.Title,
            Content = question.Content,
            Author = DtoBuilders.Users.BuildAuthorDto(author),
            Answers = DtoBuilders.Answers.BuildAnswersDto(answers, requestedBy),
            BestAnswer = bestAnswerDto,
            CreatedAt = question.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
            UpdatedAt = question.UpdatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
            AvailableToEdit = requestedBy == question.AuthorId
        };

        return dto;
    }
}
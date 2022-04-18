using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Queries.Get;

public class GetQuestionQuery : IRequest<QuestionDto>
{
    public long Id { get; }

    public GetQuestionQuery(long id)
    {
        Id = id;
    }
}

public class GetQuestionQueryHandler : IRequestHandler<GetQuestionQuery, QuestionDto>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;

    public GetQuestionQueryHandler(IDomainContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<QuestionDto> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .ThenInclude(a => a.Author)
            .Include(q => q.BestAnswer)
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

        ThrowIf.Null(question, $"There is no question with such id: {request.Id}");
        ThrowIf.Null(question!.Author, "Question author can't be null");

        var author = question!.Author;
        var answers = question.Answers.OrderByDescending(a => a.CreatedAt).ToList();
        var bestAnswer = question.BestAnswer;

        AnswerDto bestAnswerDto = null;
        var requestedBy = _userService.UserId;
        
        if (bestAnswer != null)
        {
            answers = answers.Where(a => a.Id != bestAnswer.Id).ToList();
            bestAnswerDto = Builders.Answers.BuildAnswerDto(bestAnswer, requestedBy);
        }

        var dto = new QuestionDto
        {
            Id = question.Id,
            Title = question.Title,
            Content = question.Content,
            Author = Builders.Users.BuildAuthorDto(author),
            Answers = Builders.Answers.BuildAnswersDto(answers, requestedBy),
            BestAnswer = bestAnswerDto,
            CreatedAt = question.CreatedAt.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
            AvailableToEdit = requestedBy == question.AuthorId
        };

        return dto;
    }
}
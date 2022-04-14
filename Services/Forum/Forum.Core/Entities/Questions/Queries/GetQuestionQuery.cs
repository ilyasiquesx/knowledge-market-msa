using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Queries;

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

    public GetQuestionQueryHandler(IDomainContext context)
    {
        _context = context;
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
        var answers = question.Answers;
        var bestAnswer = question.BestAnswer;
        
        AnswerDto bestAnswerDto = null;
        if (bestAnswer != null)
        {
            answers = answers.Where(a => a.Id != bestAnswer.Id).ToList();
            bestAnswerDto = Builders.Answers.BuildAnswerDto(bestAnswer);
        }

        var dto = new QuestionDto
        {
            Title = question.Title,
            Content = question.Content,
            Author = Builders.Users.BuildAuthorDto(author),
            Answers = Builders.Answers.BuildAnswersDto(answers),
            BestAnswer = bestAnswerDto,
            CreatedAt = question.CreatedAt.ToLocalTime(),
        };

        return dto;
    }
}
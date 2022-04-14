using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Answers.Commands;

public class CreateAnswerCommand : IRequest
{
    public long QuestionId { get; set; }
    public string Content { get; set; }
}

public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand>
{
    private readonly IDomainContext _context;
    private readonly IDateService _dateService;
    private readonly IIntegrationEventService _eventService;
    private readonly IUserService _userService;

    public CreateAnswerCommandHandler(IDomainContext context,
        IDateService dateService,
        IIntegrationEventService eventService, 
        IUserService userService)
    {
        _context = context;
        _dateService = dateService;
        _eventService = eventService;
        _userService = userService;
    }

    public async Task<Unit> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);
        ThrowIf.Null(question, $"There is no question with such id: {request.QuestionId}");

        var userId = _userService.UserId;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        ThrowIf.Null(user, $"There is no user with such id: {userId}");

        question!.Answers.Add(new Answer
        {
            AuthorId = user!.Id,
            Content = request.Content,
            CreatedAt = _dateService.Now
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("AnswerCreated", new
        {
            QuestionId = question.Id,
            CommentAuthorId = user.Id
        });
        return Unit.Value;
    }
}
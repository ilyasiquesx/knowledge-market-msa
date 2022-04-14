using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Commands;

public class CreateQuestionCommand : IRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand>
{
    private readonly IDomainContext _context;
    private readonly IDateService _dateService;
    private readonly IUserService _userService;
    private readonly IIntegrationEventService _eventService;

    public CreateQuestionCommandHandler(IDomainContext context, IDateService dateService, IUserService userService, IIntegrationEventService eventService)
    {
        _context = context;
        _dateService = dateService;
        _userService = userService;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Content))
            throw new Exception("Title and content must exist");

        var userId = _userService.UserId;
        var user = await _context.Users
            .Include(u => u.Questions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        ThrowIf.Null(user, $"There is no user with such id: {userId}");

        var question = new Question
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = _dateService.Now,
        };

        user!.Questions.Add(question);
        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("QuestionCreated", new
        {
            question.Id,
            question.Title,
            question.AuthorId,
            question.BestAnswerId
        });
        
        return Unit.Value;
    }
}
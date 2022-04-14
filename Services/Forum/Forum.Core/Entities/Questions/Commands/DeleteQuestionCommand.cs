using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Commands;

public class DeleteQuestionCommand : IRequest
{
    public long Id { get; }

    public DeleteQuestionCommand(long id)
    {
        Id = id;
    }
}

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand>
{
    private readonly IUserService _userService;
    private readonly IDomainContext _context;
    private readonly IIntegrationEventService _eventService;

    public DeleteQuestionCommandHandler(IUserService userService, IDomainContext context, IIntegrationEventService eventService)
    {
        _userService = userService;
        _context = context;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        ThrowIf.Null(question, $"There is no question with such id: {request.Id}");

        var userId = _userService.UserId;
        ThrowIf.NullOrEmpty(userId, "Author id must exist");
        
        var whetherUserIsAuthor = question!.Author.Id == userId;
        ThrowIf.False(whetherUserIsAuthor, "You can't delete another user's question");

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("QuestionDeleted", new
        {
            question.Id
        });
        
        return Unit.Value;
    }
}
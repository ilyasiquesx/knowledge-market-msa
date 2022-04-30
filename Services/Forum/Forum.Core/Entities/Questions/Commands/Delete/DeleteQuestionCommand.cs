using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Commands.Delete;

public class DeleteQuestionCommand : IRequest<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>>
{
    public long Id { get; }

    public DeleteQuestionCommand(long id)
    {
        Id = id;
    }
}

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand,
    OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>>
{
    private readonly IUserService _userService;
    private readonly IDomainContext _context;
    private readonly IIntegrationEventService _eventService;

    public DeleteQuestionCommandHandler(IUserService userService, IDomainContext context,
        IIntegrationEventService eventService)
    {
        _userService = userService;
        _context = context;
        _eventService = eventService;
    }

    public async Task<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>> Handle(DeleteQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        if (question == null)
            return new NotFoundResult($"There is no question with such id: {request.Id}");
        
        if (string.IsNullOrEmpty(question.AuthorId) || question.Author == null)
            return new InvalidDomainBehaviorResult("Question must have an author");
        
        var userId = _userService.UserId;
        if (string.IsNullOrEmpty(userId))
            return new InvalidDomainBehaviorResult("User id must exist");

        var whetherUserIsAuthor = question.Author.Id == userId;
        if (!whetherUserIsAuthor)
            return new InvalidDomainBehaviorResult("You can't delete another user's question");

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("QuestionDeleted", new
        {
            question.Id
        });

        return Unit.Value;
    }
}
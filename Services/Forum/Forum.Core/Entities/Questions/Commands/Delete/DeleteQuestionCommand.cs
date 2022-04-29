using Forum.Core.Entities.Questions.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Commands.Delete;

public class DeleteQuestionCommand : IRequest<OneOf<Deleted, QuestionNotFound, InvalidUserId, UserNotAllowed>>
{
    public long Id { get; }

    public DeleteQuestionCommand(long id)
    {
        Id = id;
    }
}

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, OneOf<Deleted, QuestionNotFound, InvalidUserId, UserNotAllowed>>
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

    public async Task<OneOf<Deleted, QuestionNotFound, InvalidUserId, UserNotAllowed>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        if (question == null)
            return new QuestionNotFound(request.Id);

        var userId = _userService.UserId;
        if (string.IsNullOrEmpty(userId))
            return new InvalidUserId("User id must exist");
        
        var whetherUserIsAuthor = question!.Author.Id == userId;
        if(!whetherUserIsAuthor)
            return new UserNotAllowed("You can't edit another user's question");

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("QuestionDeleted", new
        {
            question.Id
        });
        
        return new Deleted();
    }
}
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Answers.Commands.Delete;

public class DeleteAnswerCommand : IRequest
{
    public long Id { get; }

    public DeleteAnswerCommand(long id)
    {
        Id = id;
    }
}

public class DeleteAnswerCommandHandler : IRequestHandler<DeleteAnswerCommand>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;

    public DeleteAnswerCommandHandler(IDomainContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Unit> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        
        ThrowIf.Null(answer, $"There is no answer with such id: {request.Id}");
        ThrowIf.Null(answer!.AuthorId, "Answer can't have no author");

        var whetherUserIsAuthor = answer.AuthorId == _userService.UserId;
        ThrowIf.False(whetherUserIsAuthor, "You can't delete other user' answer");

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
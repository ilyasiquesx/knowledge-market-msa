using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Answers.Commands.Delete;

public class DeleteAnswerCommand : IRequest<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>>
{
    public long Id { get; }

    public DeleteAnswerCommand(long id)
    {
        Id = id;
    }
}

public class DeleteAnswerCommandHandler : IRequestHandler<DeleteAnswerCommand, OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;

    public DeleteAnswerCommandHandler(IDomainContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult>> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (answer == null)
            return new NotFoundResult($"There is no answer with such id: {request.Id}");
        
        if (string.IsNullOrEmpty(answer.AuthorId) || answer.Author == null)
            return new InvalidDomainBehaviorResult("Answer must have an author");

        var whetherUserIsAuthor = answer.AuthorId == _userService.UserId;
        if (!whetherUserIsAuthor)
            return new InvalidDomainBehaviorResult("You can't delete other user' answer");

        _context.Answers.Remove(answer);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
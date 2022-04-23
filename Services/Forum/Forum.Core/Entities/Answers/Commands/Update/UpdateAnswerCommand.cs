using System.Text.Json.Serialization;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Answers.Commands.Update;

public class UpdateAnswerCommand : IRequest
{
    public long Id { get; set; }
    public string Content { get; set; }
}

public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;
    private readonly IDateService _dateService;

    public UpdateAnswerCommandHandler(IDomainContext context, IUserService userService, 
        IDateService dateService)
    {
        _context = context;
        _userService = userService;
        _dateService = dateService;
    }

    public async Task<Unit> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        ThrowIf.Null(answer, $"There is no answer with such id: {request.Id}");
        ThrowIf.NullOrEmpty(answer!.AuthorId, $"Answer can't have no author");

        var whetherUserIsAuthor = answer.AuthorId == _userService.UserId;
        ThrowIf.False(whetherUserIsAuthor, "You can't edit other user' answer");

        answer.Content = request.Content;
        answer.UpdatedAt = _dateService.Now;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
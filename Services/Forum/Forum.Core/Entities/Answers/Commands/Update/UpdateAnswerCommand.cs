using FluentValidation;
using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Answers.Commands.Update;

public class UpdateAnswerCommand : IRequest<OneOf<Unit, ValidationResult, NotFoundResult, InvalidDomainBehaviorResult>>
{
    public long Id { get; set; }
    public string Content { get; set; }
}

public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand, OneOf<Unit, ValidationResult, NotFoundResult, InvalidDomainBehaviorResult>>
{
    private readonly IDomainContext _context;
    private readonly IUserService _userService;
    private readonly IDateService _dateService;
    private readonly IValidator<UpdateAnswerCommand> _validator;

    public UpdateAnswerCommandHandler(IDomainContext context, 
        IUserService userService, 
        IDateService dateService, 
        IValidator<UpdateAnswerCommand> validator)
    {
        _context = context;
        _userService = userService;
        _dateService = dateService;
        _validator = validator;
    }

    public async Task<OneOf<Unit, ValidationResult, NotFoundResult, InvalidDomainBehaviorResult>> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationResult(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var answer = await _context.Answers
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (answer == null)
            return new NotFoundResult($"There is no answer with such id: {request.Id}");

        if (string.IsNullOrEmpty(answer.AuthorId) || answer.Author == null)
            return new InvalidDomainBehaviorResult("Answer must have an author");

        var whetherUserIsAuthor = answer.AuthorId == _userService.UserId;
        if (!whetherUserIsAuthor)
            return new InvalidDomainBehaviorResult("You can't edit other user' answer");

        answer.Content = request.Content;
        answer.UpdatedAt = _dateService.Now;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
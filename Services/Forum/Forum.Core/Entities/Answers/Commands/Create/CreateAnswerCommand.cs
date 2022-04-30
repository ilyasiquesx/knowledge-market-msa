using FluentValidation;
using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Answers.Commands.Create;

public class CreateAnswerCommand : IRequest<OneOf<Unit, ValidationResult, NotFoundResult>>
{
    public long QuestionId { get; set; }
    public string Content { get; set; }
}

public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, OneOf<Unit, ValidationResult, NotFoundResult>>
{
    private readonly IDomainContext _context;
    private readonly IDateService _dateService;
    private readonly IIntegrationEventService _eventService;
    private readonly IUserService _userService;
    private readonly IValidator<CreateAnswerCommand> _validator;

    public CreateAnswerCommandHandler(IDomainContext context,
        IDateService dateService,
        IIntegrationEventService eventService,
        IUserService userService, 
        IValidator<CreateAnswerCommand> validator)
    {
        _context = context;
        _dateService = dateService;
        _eventService = eventService;
        _userService = userService;
        _validator = validator;
    }

    public async Task<OneOf<Unit, ValidationResult, NotFoundResult>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationResult(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);
        if (question == null)
            return new NotFoundResult($"There is no question with such id: {request.QuestionId}");

        var userId = _userService.UserId;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
            return new NotFoundResult($"There is no user with such user id: {userId}");

        question.Answers.Add(new Answer
        {
            AuthorId = user.Id,
            Content = request.Content,
            CreatedAt = _dateService.Now,
            UpdatedAt = _dateService.Now
        });
        
        question.UpdatedAt = _dateService.Now;

        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("AnswerCreated", new
        {
            QuestionId = question.Id,
            CommentAuthorId = user.Id
        });
        
        return Unit.Value;
    }
}
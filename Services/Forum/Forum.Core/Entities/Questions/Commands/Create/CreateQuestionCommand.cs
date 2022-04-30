using FluentValidation;
using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Commands.Create;

public class CreateQuestionCommand : IRequest<OneOf<Unit, ValidationResult, NotFoundResult>>
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class
    CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, OneOf<Unit, ValidationResult, NotFoundResult>>
{
    private readonly IDomainContext _context;
    private readonly IDateService _dateService;
    private readonly IUserService _userService;
    private readonly IIntegrationEventService _eventService;
    private readonly IValidator<CreateQuestionCommand> _validator;

    public CreateQuestionCommandHandler(IDomainContext context, 
        IDateService dateService, 
        IUserService userService,
        IIntegrationEventService eventService, 
        IValidator<CreateQuestionCommand> validator)
    {
        _context = context;
        _dateService = dateService;
        _userService = userService;
        _eventService = eventService;
        _validator = validator;
    }

    public async Task<OneOf<Unit, ValidationResult, NotFoundResult>> Handle(CreateQuestionCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationResult(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var userId = _userService.UserId;
        var user = await _context.Users
            .Include(u => u.Questions)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return new NotFoundResult($"There is no user with user id: {userId}");

        var question = new Question
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = _dateService.Now,
            UpdatedAt = _dateService.Now
        };

        user.Questions.Add(question);
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
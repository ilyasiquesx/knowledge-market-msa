using System.Text.Json.Serialization;
using FluentValidation;
using Forum.Core.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Commands.Update;

public class UpdateQuestionCommand : IRequest<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult, ValidationResult>>
{
    [JsonIgnore]
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long? BestAnswerId { get; set; }
}

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult, ValidationResult>>
{
    private readonly IUserService _userService;
    private readonly IDomainContext _context;
    private readonly IIntegrationEventService _eventService;
    private readonly IDateService _dateService;
    private readonly IValidator<UpdateQuestionCommand> _validator;

    public UpdateQuestionCommandHandler(IUserService userService, 
        IDomainContext context, 
        IIntegrationEventService eventService, 
        IDateService dateService, 
        IValidator<UpdateQuestionCommand> validator)
    {
        _userService = userService;
        _context = context;
        _eventService = eventService;
        _dateService = dateService;
        _validator = validator;
    }

    public async Task<OneOf<Unit, NotFoundResult, InvalidDomainBehaviorResult, ValidationResult>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return new ValidationResult(validationResult.Errors.Select(e => e.ErrorMessage));
        
        var question = await _context.Questions
            .Include(q => q.Author)
            .Include(q => q.Answers)
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
            return new InvalidDomainBehaviorResult("You can't edit another user's question");

        question.Title = request.Title;
        question.Content = request.Content;
        question.BestAnswerId = request.BestAnswerId;
        question.UpdatedAt = _dateService.Now;

        await _context.SaveChangesAsync(cancellationToken);
        await _eventService.Publish("QuestionUpdated", new
        {
            question.Id,
            question.Title,
            question.BestAnswerId,
        });
        
        return Unit.Value;
    }
}
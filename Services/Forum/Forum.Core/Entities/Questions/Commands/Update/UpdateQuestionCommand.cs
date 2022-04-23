using System.Text.Json.Serialization;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Entities.Questions.Commands.Update;

public class UpdateQuestionCommand : IRequest
{
    [JsonIgnore]
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long? BestAnswerId { get; set; }
}

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand>
{
    private readonly IUserService _userService;
    private readonly IDomainContext _context;
    private readonly IIntegrationEventService _eventService;
    private readonly IDateService _dateService;

    public UpdateQuestionCommandHandler(IUserService userService, 
        IDomainContext context, 
        IIntegrationEventService eventService, 
        IDateService dateService)
    {
        _userService = userService;
        _context = context;
        _eventService = eventService;
        _dateService = dateService;
    }

    public async Task<Unit> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        
        ThrowIf.Null(question, $"There is no question with such id: {request.Id}");

        var userId = _userService.UserId;
        ThrowIf.NullOrEmpty(userId, "User id must exist");

        var isUserQuestionAuthor = question!.Author.Id == userId;
        ThrowIf.False(isUserQuestionAuthor, "You can't edit another user's question");

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
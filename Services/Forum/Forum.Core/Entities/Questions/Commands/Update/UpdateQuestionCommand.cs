using System.Text.Json.Serialization;
using Forum.Core.Entities.Questions.Results;
using Forum.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Forum.Core.Entities.Questions.Commands.Update;

public class UpdateQuestionCommand : IRequest<OneOf<Updated, QuestionNotFound, InvalidUserId, UserNotAllowed>>
{
    [JsonIgnore]
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long? BestAnswerId { get; set; }
}

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, OneOf<Updated, QuestionNotFound, InvalidUserId, UserNotAllowed>>
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

    public async Task<OneOf<Updated, QuestionNotFound, InvalidUserId, UserNotAllowed>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _context.Questions
            .Include(q => q.Author)
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
        if (question == null)
            return new QuestionNotFound(request.Id);

        var userId = _userService.UserId;
        if (string.IsNullOrEmpty(userId))
            return new InvalidUserId("User id must exist");
        
        var whetherUserIsAuthor = question!.Author.Id == userId;
        if (!whetherUserIsAuthor)
            return new UserNotAllowed("You can't edit another user's question");

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
        
        return new Updated();
    }
}
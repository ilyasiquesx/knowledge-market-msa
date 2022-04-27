using FluentValidation;

namespace Forum.Core.Entities.Questions.Commands.Update;

public class UpdateCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateCommandValidator()
    {
        RuleFor(q => q.Title).NotEmpty().WithMessage("Question title must exist");
        RuleFor(q => q.Content).NotEmpty().WithMessage("Question content must exist");
    }
}
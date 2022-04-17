using FluentValidation;

namespace Forum.Core.Entities.Questions.Commands.Update;

public class UpdateCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateCommandValidator()
    {
        RuleFor(q => q.Title).NotEmpty().WithMessage("Title must exist");
        RuleFor(q => q.Content).NotEmpty().WithMessage("Content must exist");
    }
}
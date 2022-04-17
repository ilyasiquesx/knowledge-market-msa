using FluentValidation;

namespace Forum.Core.Entities.Questions.Commands.Create;

public class CreateCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(q => q.Title).NotEmpty().WithMessage("Title must exist");
        RuleFor(q => q.Content).NotEmpty().WithMessage("Content must exist");
    }
}
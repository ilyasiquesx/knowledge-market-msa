using FluentValidation;

namespace Forum.Core.Entities.Questions.Commands.Create;

public class CreateCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateCommandValidator()
    {
        RuleFor(q => q.Title).NotEmpty().WithMessage("Question title must exist");
        RuleFor(q => q.Content).NotEmpty().WithMessage("Question content must exist");
    }
}
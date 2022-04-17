using FluentValidation;

namespace Forum.Core.Entities.Answers.Commands.Create;

public class CreateAnswerValidator : AbstractValidator<CreateAnswerCommand>
{
    public CreateAnswerValidator()
    {
        RuleFor(a => a.Content).NotEmpty().WithMessage("Answer content must exist");
    }
}
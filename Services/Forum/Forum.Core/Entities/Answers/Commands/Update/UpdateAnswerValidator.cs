using FluentValidation;

namespace Forum.Core.Entities.Answers.Commands.Update;

public class UpdateAnswerValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerValidator()
    {
        RuleFor(a => a.Content).NotEmpty().WithMessage("Answer content must exist");
    }
}
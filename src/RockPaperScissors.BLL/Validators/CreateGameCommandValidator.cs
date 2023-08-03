using FluentValidation;

namespace RockPaperScissors.BLL.Validators;

public class CreateGameCommandValidator : AbstractValidator<CreateGameCommand>
{
    public CreateGameCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty();
    }
}
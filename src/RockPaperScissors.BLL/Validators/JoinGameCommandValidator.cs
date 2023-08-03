using FluentValidation;

namespace RockPaperScissors.BLL.Validators;

public class JoinGameCommandValidator : AbstractValidator<JoinGameCommand>
{
    public JoinGameCommandValidator()
    {
        RuleFor(c => c.GameId)
            .GreaterThan(0);

        RuleFor(c => c.UserName)
            .NotEmpty();
    }
}
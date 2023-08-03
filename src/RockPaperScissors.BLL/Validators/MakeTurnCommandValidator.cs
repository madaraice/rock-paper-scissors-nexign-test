using FluentValidation;

namespace RockPaperScissors.BLL.Validators;

public class MakeTurnCommandValidator : AbstractValidator<MakeTurnCommand>
{
    public MakeTurnCommandValidator()
    {
        RuleFor(c => c.UserId)
            .GreaterThan(0);
        
        RuleFor(c => c.GameId)
            .GreaterThan(0);

        RuleFor(c => c.Turn)
            .Must(c => c is not Turn.Unknown);
    }
}
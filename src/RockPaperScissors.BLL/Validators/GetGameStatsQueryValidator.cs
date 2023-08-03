using FluentValidation;
using RockPaperScissors.BLL.Queries;

namespace RockPaperScissors.BLL.Validators;

public class GetGameStatsQueryValidator : AbstractValidator<GetGameStatsQuery>
{
    public GetGameStatsQueryValidator()
    {
        RuleFor(q => q.GameId)
            .GreaterThan(0);
    }
}
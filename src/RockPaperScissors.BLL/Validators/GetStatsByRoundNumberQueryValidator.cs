using FluentValidation;
using RockPaperScissors.BLL.Queries;

namespace RockPaperScissors.BLL.Validators;

public class GetStatsByRoundNumberQueryValidator : AbstractValidator<GetStatsByRoundNumberQuery>
{
    public GetStatsByRoundNumberQueryValidator()
    {
        RuleFor(q => q.GameId)
            .GreaterThan(0);
        
        RuleFor(q => q.RoundNumber)
            .GreaterThan(0);
    }
}
namespace RockPaperScissors.BLL.Queries;

public record GetStatsByRoundNumberQuery : IRequest<GetStatsByRoundNumberResult>
{
    public required long GameId { get; init; }
    public required long RoundNumber { get; init; }
}

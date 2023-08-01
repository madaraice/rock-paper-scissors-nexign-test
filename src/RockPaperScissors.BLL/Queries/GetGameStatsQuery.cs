namespace RockPaperScissors.BLL.Queries;

public record GetGameStatsQuery : IRequest<GetGameStatsResult>
{
    public required long GameId { get; init; }
}

using RockPaperScissors.BLL.Model;

namespace RockPaperScissors.BLL.Results;

public record GetGameStatsResult
{
    // null будет в случае ничьи
    public required long? WinnerUserId { get; init; }
    public required IReadOnlyList<UserStatWithRoundResult> FirstUserStats { get; init; }
    public required IReadOnlyList<UserStatWithRoundResult> SecondUserStats { get; init; }
}
using RockPaperScissors.BLL.Model;

namespace RockPaperScissors.BLL.Results;

public record GetGameStatsResult
{
    // null будет в случае ничьи
    public required long? WinnerUserId { get; init; }
    public required string? WinnerUserName { get; init; }
    public required UserInfoWithRoundResult FirstUserStats { get; init; }
    public required UserInfoWithRoundResult SecondUserStats { get; init; }
}
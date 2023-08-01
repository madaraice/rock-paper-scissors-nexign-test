using RockPaperScissors.BLL.Model;

namespace RockPaperScissors.Api.Models.Responses;

public record GetGameStatsResponse
{
    public required long? WinnerUserId { get; init; }
    public required IReadOnlyList<UserStatWithRoundResult> FirstUserStats { get; init; }
    public required IReadOnlyList<UserStatWithRoundResult> SecondUserStats { get; init; }
}
using RockPaperScissors.BLL.Model;

namespace RockPaperScissors.Api.Models.Responses;

public record GetStatsByRoundNumberResponse
{
    public required long? WinnerUserId { get; init; }
    public required string? WinnerUserName { get; init; }
    public required UserInfoWithRoundResult FirstUserStats { get; init; }
    public required UserInfoWithRoundResult SecondUserStats { get; init; }
}
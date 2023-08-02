namespace RockPaperScissors.BLL.Model;

public record UserInfoWithRoundResult
{
    public required long UserId { get; init; }
    public required string UserName { get; init; }
    public required IReadOnlyList<RoundResultByUser> RoundResults { get; init; }

    public record RoundResultByUser
    {
        public required long RoundNumber { get; init; }
        public required Turn Turn { get; init; }
        public required RoundResult RoundResult { get; init; }
    }
}
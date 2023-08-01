namespace RockPaperScissors.BLL.Model;

public record UserStat
{
    public required long UserId { get; init; }
    public required string UserName { get; init; }
    public required long RoundNumber { get; init; }
    public required Turn Turn { get; init; }
}
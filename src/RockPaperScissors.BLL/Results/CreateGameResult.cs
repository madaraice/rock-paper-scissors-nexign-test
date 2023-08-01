namespace RockPaperScissors.BLL.Results;

public record CreateGameResult
{
    public required long UserId { get; init; }
    public required long GameId { get; init; }
}
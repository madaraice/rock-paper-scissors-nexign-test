namespace RockPaperScissors.DAL.Models;

public class GameUser
{
    public long Id { get; init; }
    public required long GameId { get; init; }
    public required long UserId { get; init; }
}
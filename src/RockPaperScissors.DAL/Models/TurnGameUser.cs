namespace RockPaperScissors.DAL.Models;

public class TurnGameUser
{
    public required long GameUserId { get; init; }
    public required int RoundNumber { get; init; }
    public required int Turn { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}    

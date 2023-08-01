namespace RockPaperScissors.DAL.Models;

public class TurnGameUser
{
    public required long GameUserId { get; init; }
    // todo rename RoundNumber
    public required int TurnNumber { get; init; }
    // todo rename Turn
    public required int TurnValue { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
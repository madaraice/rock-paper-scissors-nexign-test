namespace RockPaperScissors.DAL.Models;

public class Game
{
    public long Id { get; init; }
    public required int State { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
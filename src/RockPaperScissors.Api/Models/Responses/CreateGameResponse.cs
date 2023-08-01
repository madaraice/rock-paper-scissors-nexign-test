namespace RockPaperScissors.Api.Models.Responses;

public record CreateGameResponse
{
    public required long UserId { get; init; }
    public required long GameId { get; init; }
}
namespace RockPaperScissors.Api.Models.Responses;

public record JoinGameResponse
{
    public required long UserId { get; init; }
}
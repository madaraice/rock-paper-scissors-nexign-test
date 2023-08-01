namespace RockPaperScissors.Api.Models.Requests;

public record CreateGameRequest
{
    public required string UserName { get; init; }
}
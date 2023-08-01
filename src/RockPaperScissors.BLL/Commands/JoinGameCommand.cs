namespace RockPaperScissors.BLL.Commands;

public record JoinGameCommand : IRequest<JoinGameResult>
{
    public required long GameId { get; init; }
    public required string UserName { get; init; }
}
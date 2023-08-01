namespace RockPaperScissors.BLL.Commands;

public record CreateGameCommand : IRequest<CreateGameResult>
{
    public required string UserName { get; init; }
}
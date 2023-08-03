namespace RockPaperScissors.BLL.Commands;

public record MakeTurnCommand : IRequest<EmptyResult>
{
    public required long GameId { get; init; }
    public required long UserId { get; init; }
    public required Turn Turn { get; init; }
}
namespace RockPaperScissors.BLL.Exceptions;

public class GameHasInvalidStateException : BusinessLogicException
{
    public GameState ExpectedState { get; }
    public GameState ActualState { get; }

    public GameHasInvalidStateException(GameState expectedState, GameState actualState)
    {
        ExpectedState = expectedState;
        ActualState = actualState;
    }
}
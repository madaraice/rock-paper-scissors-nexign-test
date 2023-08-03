namespace RockPaperScissors.BLL.Exceptions;

public class GameHasInvalidStatesException : BusinessLogicException
{
    public GameState[] ExpectedStates { get; }
    public GameState ActualState { get; }

    public GameHasInvalidStatesException(GameState[] expectedStates, GameState actualState)
    {
        ExpectedStates = expectedStates;
        ActualState = actualState;
    }
}
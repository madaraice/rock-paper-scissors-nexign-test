using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Extensions;

public static class GameExtensions
{
    public static Game EnsureNotNull(this Game? game)
    {
        if (game is null)
        {
            throw new GameNotFoundException();
        }

        return game;
    }

    public static Game EnsureHasState(this Game game, GameState expectedState)
    {
        if (game.State != (int) expectedState)
        {
            throw new GameHasInvalidStateException(expectedState, (GameState) game.State);
        }

        return game;
    }

    public static Game EnsureHasStates(this Game game, GameState[] expectedStates)
    {
        if (!expectedStates.Contains((GameState) game.State))
        {
            throw new GameHasInvalidStatesException(expectedStates, (GameState) game.State);
        }

        return game;
    }
}
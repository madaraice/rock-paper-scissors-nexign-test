using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Extensions;

public static class GameExtensions
{
    public static Game EnsureNotNull(this Game? game)
    {
        if (game is null)
        {
            // todo game not found
            throw new GameNotFoundException();
        }

        return game;
    }
}
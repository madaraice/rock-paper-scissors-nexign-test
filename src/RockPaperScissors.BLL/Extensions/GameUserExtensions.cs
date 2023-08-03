using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Extensions;

public static class GameUserExtensions
{
    public static GameUser EnsureNotNull(this GameUser? gameUser)
    {
        if (gameUser is null)
        {
            throw new UserNotJoinedInGameException();
        }

        return gameUser;
    } 
}
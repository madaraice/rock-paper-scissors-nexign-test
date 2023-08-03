using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Extensions;

public static class UserExtensions
{
    public static User EnsureNotNull(this User? user)
    {
        if (user is null)
        {
            throw new UserNotFoundException();
        }

        return user;
    } 
}
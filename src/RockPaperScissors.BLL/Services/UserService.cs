using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Services.Interfaces;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _applicationDbContext;

    public UserService(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Если юзер с таким ником уже есть, то нового можно не создавать
    /// </summary>
    public async Task<User> GetOrAdd(string userName, CancellationToken cancellationToken)
    {
        var user = await _applicationDbContext
            .Users
            .SingleOrDefaultAsync(
                u => u.UserName == userName,
                cancellationToken);

        if (user is not null)
        {
            return user;
        }

        return _applicationDbContext
            .Users
            .Add(new User { UserName = userName })
            .Entity;
    }
}
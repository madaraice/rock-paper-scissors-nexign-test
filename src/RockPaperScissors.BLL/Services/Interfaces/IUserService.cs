using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Services.Interfaces;

public interface IUserService
{
    Task<User> GetOrAdd(string userName, CancellationToken token);
}
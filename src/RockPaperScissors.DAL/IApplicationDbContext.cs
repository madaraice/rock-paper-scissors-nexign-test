using Microsoft.EntityFrameworkCore;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Game> Games { get; }
    DbSet<GameUser> GameUsers { get; }
    DbSet<TurnGameUser> TurnGameUsers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
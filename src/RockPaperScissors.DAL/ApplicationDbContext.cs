using Microsoft.EntityFrameworkCore;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameUser> GameUsers => Set<GameUser>();
    public DbSet<TurnGameUser> TurnGameUsers => Set<TurnGameUser>();

    public ApplicationDbContext()
    {
        
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
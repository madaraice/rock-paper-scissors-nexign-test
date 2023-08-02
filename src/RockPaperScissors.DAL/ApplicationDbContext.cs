using Microsoft.EntityFrameworkCore;
using RockPaperScissors.DAL.Configuration;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameUser> GameUsers => Set<GameUser>();
    public DbSet<TurnGameUser> TurnGameUsers => Set<TurnGameUser>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
        // Миграции лень подключать =)
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Связи между таблицами намеренно не делаю тк было мало времени на реализацию =(
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new GameUserConfiguration());
        modelBuilder.ApplyConfiguration(new TurnGameUserConfiguration());
    }
}
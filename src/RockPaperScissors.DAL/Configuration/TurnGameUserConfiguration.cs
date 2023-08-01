using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL.Configuration;

public class TurnGameUserConfiguration : IEntityTypeConfiguration<TurnGameUser>
{
    public void Configure(EntityTypeBuilder<TurnGameUser> builder)
    {
        builder.HasKey(p => new { p.GameUserId, p.RoundNumber });
    }
}
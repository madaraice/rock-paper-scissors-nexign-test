using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL.Configuration;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseHiLo();
    }
}
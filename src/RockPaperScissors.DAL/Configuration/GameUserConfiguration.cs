using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.DAL.Configuration;

public class GameUserConfiguration : IEntityTypeConfiguration<GameUser>
{
    public void Configure(EntityTypeBuilder<GameUser> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
    }
}
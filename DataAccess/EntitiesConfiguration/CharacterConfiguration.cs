using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder
            .HasKey(c => c.Id);

        builder
            .Property(c => c.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(c => c.Race)
            .HasDefaultValue(CharacterRace.Human);

        builder
            .Property(c => c.Health)
            .HasDefaultValue(100);

        builder
            .Property(c => c.PlayerId)
            .IsRequired();
    }
}

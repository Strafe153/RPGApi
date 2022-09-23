using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class SpellConfiguration : IEntityTypeConfiguration<Spell>
{
    public void Configure(EntityTypeBuilder<Spell> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(s => s.Type)
            .HasDefaultValue(SpellType.Fire);

        builder
            .Property(s => s.Damage)
            .HasDefaultValue(15);
    }
}

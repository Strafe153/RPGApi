using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class CharacterSpellConfiguration : IEntityTypeConfiguration<CharacterSpell>
{
    public void Configure(EntityTypeBuilder<CharacterSpell> builder)
    {
        builder
            .HasKey(cw => new
            {
                cw.CharacterId,
                cw.SpellId
            });

        builder
            .HasOne(cw => cw.Character)
            .WithMany(cw => cw.CharacterSpells)
            .HasForeignKey(cw => cw.CharacterId);

        builder
            .HasOne(cw => cw.Spell)
            .WithMany(cw => cw.CharacterSpells)
            .HasForeignKey(cw => cw.SpellId);

        builder
            .ToTable("CharacterSpells");
    }
}

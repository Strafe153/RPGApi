using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class CharacterWeaponConfiguration : IEntityTypeConfiguration<CharacterWeapon>
{
    public void Configure(EntityTypeBuilder<CharacterWeapon> builder)
    {
        builder
            .HasKey(cw => new
            {
                cw.CharacterId,
                cw.WeaponId
            });

        builder
            .HasOne(cw => cw.Character)
            .WithMany(cw => cw.CharacterWeapons)
            .HasForeignKey(cw => cw.CharacterId);

        builder
            .HasOne(cw => cw.Weapon)
            .WithMany(cw => cw.CharacterWeapons)
            .HasForeignKey(cw => cw.WeaponId);

        builder
            .ToTable("CharacterWeapons");
    }
}

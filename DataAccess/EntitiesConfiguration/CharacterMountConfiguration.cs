using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class CharacterMountConfiguration : IEntityTypeConfiguration<CharacterMount>
{
    public void Configure(EntityTypeBuilder<CharacterMount> builder)
    {
        builder
            .HasKey(cw => new
            {
                cw.CharacterId,
                cw.MountId
            });

        builder
            .HasOne(cw => cw.Character)
            .WithMany(cw => cw.CharacterMounts)
            .HasForeignKey(cw => cw.CharacterId);

        builder
            .HasOne(cw => cw.Mount)
            .WithMany(cw => cw.CharacterMounts)
            .HasForeignKey(cw => cw.MountId);

        builder
            .ToTable("CharacterMounts");
    }
}

using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        builder
            .HasKey(w => w.Id);

        builder
            .Property(w => w.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(w => w.Type)
            .HasDefaultValue(WeaponType.Sword);

        builder
            .Property(w => w.Damage)
            .HasDefaultValue(30);
    }
}

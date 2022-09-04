using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder
                .HasKey(p => p.Id);

            builder
                .HasIndex(p => p.Name)
                .IsUnique();

            builder
                .Property(p => p.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(p => p.Role)
                .HasDefaultValue(PlayerRole.Player);
        }
    }
}

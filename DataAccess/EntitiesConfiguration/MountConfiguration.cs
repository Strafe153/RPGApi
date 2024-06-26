﻿using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntitiesConfiguration;

public class MountConfiguration : IEntityTypeConfiguration<Mount>
{
    public void Configure(EntityTypeBuilder<Mount> builder)
    {
        builder
            .HasKey(m => m.Id);

        builder
            .Property(m => m.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder
            .Property(m => m.Type)
            .HasDefaultValue(MountType.Horse);

        builder
            .Property(m => m.Speed)
            .HasDefaultValue(10);
    }
}

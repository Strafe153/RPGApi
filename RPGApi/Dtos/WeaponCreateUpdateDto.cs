﻿using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record WeaponCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; init; }

        public WeaponType Type { get; init; } = WeaponType.Sword;

        [Required]
        public Guid CharacterId { get; init; }
    }
}

using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos.Weapons
{
    public record WeaponCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }

        public WeaponType Type { get; init; } = WeaponType.Sword;

        [Range(1, 100)]
        public int? Damage { get; init; } = 30;
    }
}

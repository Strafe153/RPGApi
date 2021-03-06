using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos.Spells
{
    public record SpellCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }

        public SpellType Type { get; init; } = SpellType.Fire;

        [Range(-100, 100)]
        public int Damage { get; init; } = 15;
    }
}

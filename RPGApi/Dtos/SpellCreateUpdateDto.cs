using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record SpellCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; init; }

        public SpellType Type { get; init; }
        public int Damage { get; init; }
    }
}

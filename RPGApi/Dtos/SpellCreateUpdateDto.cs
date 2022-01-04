using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record SpellCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; set; }

        public SpellType Type { get; set; } = SpellType.Fire;
        public int Damage { get; set; } = 15;

        [Required]
        public Guid CharacterId { get; set; }
    }
}

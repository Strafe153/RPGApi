using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos.Characters
{
    public record CharacterCreateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string? Name { get; init; }

        public CharacterRace Race { get; init; } = CharacterRace.Human;

        [Required]
        public Guid PlayerId { get; init; }
    }
}

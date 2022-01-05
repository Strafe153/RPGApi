using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record CharacterUpdateDto
    {
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; init; }

        public CharacterRace Race { get; init; } = CharacterRace.Human;
    }
}

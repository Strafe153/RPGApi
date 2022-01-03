using System.ComponentModel.DataAnnotations;
using RPGApi.Data;

namespace RPGApi.Dtos
{
    public class CharacterCreateUpdateDto
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Name { get; set; }

        public CharacterRace Race { get; set; } = CharacterRace.Human;

        [Required]
        public Guid PlayerId { get; set; }
    }
}

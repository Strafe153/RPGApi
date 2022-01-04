using RPGApi.Data;
using RPGApi.Models;

namespace RPGApi.Dtos
{
    public record CharacterReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public CharacterRace Race { get; init; }
        public int Health { get; init; }

        public ICollection<Weapon> Weapons { get; init; }
        public ICollection<Spell> Spells { get; init; }

        public Guid PlayerId { get; init; }
    }
}

using RPGApi.Data;

namespace RPGApi.Dtos.Characters
{
    public record CharacterReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public CharacterRace Race { get; init; }
        public int Health { get; init; }

        public ICollection<Weapon>? Weapons { get; init; }
        public ICollection<Spell>? Spells { get; init; }
        public ICollection<Mount>? Mounts { get; init; }

        public Guid PlayerId { get; init; }
    }
}

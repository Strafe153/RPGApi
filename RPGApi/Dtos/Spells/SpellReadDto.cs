using RPGApi.Data;

namespace RPGApi.Dtos.Spells
{
    public record SpellReadDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public SpellType Type { get; init; }
        public int Damage { get; init; }

        public ICollection<Character>? Characters { get; init; }
    }
}

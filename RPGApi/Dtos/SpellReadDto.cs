using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record SpellReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public SpellType Type { get; init; }
        public int Damage { get; init; }

        public Guid CharacterId { get; init; }
    }
}

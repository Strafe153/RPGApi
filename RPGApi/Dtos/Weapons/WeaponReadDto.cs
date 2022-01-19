using RPGApi.Data;

namespace RPGApi.Dtos.Weapons
{
    public record WeaponReadDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public WeaponType Type { get; init; }
        public int Damage { get; init; }

        public ICollection<Character>? Characters { get; init; }
    }
}

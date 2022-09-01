using Core.Entities;

namespace Core.Dtos.WeaponDtos
{
    public record WeaponReadDto : WeaponBaseDto
    {
        public int Id { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}

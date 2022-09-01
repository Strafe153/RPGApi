using Core.Entities;

namespace Core.Dtos.SpellDtos
{
    public record SpellReadDto : SpellBaseDto
    {
        public int Id { get; init; }
        public IEnumerable<Character>? Characters { get; init; }
    }
}

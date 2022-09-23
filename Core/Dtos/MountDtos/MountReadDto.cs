using Core.Entities;

namespace Core.Dtos.MountDtos;

public record MountReadDto : MountBaseDto
{
    public int Id { get; init; }
    public IEnumerable<Character>? Characters { get; init; }
}

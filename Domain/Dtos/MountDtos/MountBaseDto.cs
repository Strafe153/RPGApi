using Domain.Enums;

namespace Domain.Dtos.MountDtos;

public record MountBaseDto
{
    public string? Name { get; init; }
    public MountType Type { get; init; }
    public int Speed { get; init; }
}

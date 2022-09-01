using Core.Enums;

namespace Core.Dtos.MountDtos
{
    public record MountBaseDto
    {
        public string? Name { get; init; }
        public MountType Type { get; init; }
        public int Speed { get; init; }
    }
}

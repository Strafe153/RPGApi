using RPGApi.Data;

namespace RPGApi.Dtos.Mounts
{
    public record MountCreateUpdateDto
    {
        public string Name { get; init; }
        public MountType Type { get; init; } = MountType.Horse;
        public int Speed { get; init; } = 8;
    }
}

using RPGApi.Data;

namespace RPGApi.Dtos
{
    public record MountCreateUpdateDto
    {
        public string Name { get; init; }
        public MountType Type { get; init; } = MountType.Horse;
        public int Speed { get; init; } = 8;
    }
}

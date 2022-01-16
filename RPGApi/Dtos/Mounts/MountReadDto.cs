using RPGApi.Data;

namespace RPGApi.Dtos.Mounts
{
    public record MountReadDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public MountType Type { get; init; }
        public int Health { get; init; }
        public int Speed { get; init; }

        public ICollection<Character> Characters { get; init; }
    }
}

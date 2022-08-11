using Core.Enums;

namespace Core.Entities
{
    public class Mount
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public MountType Type { get; set; }
        public int Speed { get; set; }

        public IEnumerable<CharacterMount> CharacterMounts { get; set; } = new List<CharacterMount>();
    }
}

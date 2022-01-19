using Newtonsoft.Json;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Mount
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public MountType Type { get; set; }
        public int Health { get; set; } = 100;
        public int Speed { get; set; }

        [JsonIgnore]
        public ICollection<Character>? Characters { get; set; }
    }
}

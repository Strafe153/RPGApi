using System.Text.Json.Serialization;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Spell
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SpellType Type { get; set; } = SpellType.Fire;
        public int Damage { get; set; } = 15;

        [JsonIgnore]
        public ICollection<Character> Characters { get; set; }
    }
}

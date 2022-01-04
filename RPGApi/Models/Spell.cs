using System.Text.Json.Serialization;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Spell
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SpellType Type { get; set; }
        public int Damage { get; set; }

        [JsonIgnore]
        public Character Character { get; set; }
        public Guid CharacterId { get; set; }
    }
}

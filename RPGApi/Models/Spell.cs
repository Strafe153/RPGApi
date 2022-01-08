using Newtonsoft.Json;
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
        public ICollection<Character> Characters { get; set; }
    }
}

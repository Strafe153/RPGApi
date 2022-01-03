using System.Text.Json.Serialization;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CharacterRace Race { get; set; }
        public int Health { get; set; } = 100;

        [JsonIgnore]
        public Player Player { get; set; }
        public Guid PlayerId { get; set; }
    }
}

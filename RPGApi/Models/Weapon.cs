using System.Text.Json.Serialization;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Weapon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WeaponType Type { get; set; }
        public int Damage { get; set; } = 100;

        [JsonIgnore]
        public Character Character { get; set; }
        public Guid CharacterId { get; set; }
    }
}

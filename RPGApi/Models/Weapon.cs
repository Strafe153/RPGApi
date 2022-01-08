using Newtonsoft.Json;
using RPGApi.Data;

namespace RPGApi.Models
{
    public class Weapon
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WeaponType Type { get; set; } = WeaponType.Sword;
        public int Damage { get; set; } = 30;

        [JsonIgnore]
        public ICollection<Character> Characters { get; set; }
    }
}

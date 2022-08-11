using Core.Enums;

namespace Core.Entities
{
    public class Weapon
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public WeaponType Type { get; set; }
        public int Damage { get; set; }

        public IEnumerable<CharacterWeapon> CharacterWeapons { get; set; } = new List<CharacterWeapon>();
    }
}

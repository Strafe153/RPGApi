using Domain.Enums;

namespace Domain.Entities;

public class Weapon
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public WeaponType Type { get; set; }
    public int Damage { get; set; }

    public ICollection<CharacterWeapon> CharacterWeapons { get; set; } = new List<CharacterWeapon>();
}

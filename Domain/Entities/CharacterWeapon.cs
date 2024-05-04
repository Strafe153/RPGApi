namespace Domain.Entities;

public class CharacterWeapon
{
    public int CharacterId { get; set; }
    public Character? Character { get; set; }

    public int WeaponId { get; set; }
    public Weapon? Weapon { get; set; }
}

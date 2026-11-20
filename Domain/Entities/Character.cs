using Domain.Enums;

namespace Domain.Entities;

public class Character
{
	public int Id { get; set; }
	public string Name { get; set; } = default!;
	public CharacterRace Race { get; set; }
	public int Health { get; set; } = 100;

	public int PlayerId { get; set; }
	public Player Player { get; set; } = default!;


	public ICollection<CharacterWeapon> CharacterWeapons { get; set; } = [];
	public ICollection<CharacterSpell> CharacterSpells { get; set; } = [];
	public ICollection<CharacterMount> CharacterMounts { get; set; } = [];

	public void CalculateHealth(int damage)
    {
        if (Health - damage > 100)
        {
            Health = 100;
			return;
        }

        if (Health < damage)
        {
            Health = 0;
			return;
        }

		Health -= damage;
    }
}

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


	public ICollection<CharacterWeapon> CharacterWeapons { get; set; } = new List<CharacterWeapon>();
	public ICollection<CharacterSpell> CharacterSpells { get; set; } = new List<CharacterSpell>();
	public ICollection<CharacterMount> CharacterMounts { get; set; } = new List<CharacterMount>();
}

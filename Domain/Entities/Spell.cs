using Domain.Enums;

namespace Domain.Entities;

public class Spell
{
	public int Id { get; set; }
	public string Name { get; set; } = default!;
	public SpellType Type { get; set; }
	public int Damage { get; set; }

	public ICollection<CharacterSpell> CharacterSpells { get; set; } = new List<CharacterSpell>();
}

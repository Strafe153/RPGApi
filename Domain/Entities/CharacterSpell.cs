﻿namespace Domain.Entities;

public class CharacterSpell
{
	public int CharacterId { get; set; }
	public Character Character { get; set; } = default!;

	public int SpellId { get; set; }
	public Spell Spell { get; set; } = default!;
}

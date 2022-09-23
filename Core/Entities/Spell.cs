using Core.Enums;

namespace Core.Entities;

public class Spell
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public SpellType Type { get; set; }
    public int Damage { get; set; }

    public ICollection<CharacterSpell> CharacterSpells { get; set; } = new List<CharacterSpell>();
}

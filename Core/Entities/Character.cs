using Core.Enums;

namespace Core.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public CharacterRace Race { get; set; }
        public int Health { get; set; } = 100;

        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public IEnumerable<CharacterWeapon> CharacterWeapons { get; set; } = new List<CharacterWeapon>();
        public IEnumerable<CharacterSpell> CharacterSpells { get; set; } = new List<CharacterSpell>();
        public IEnumerable<CharacterMount> CharacterMounts { get; set; } = new List<CharacterMount>();
    }
}

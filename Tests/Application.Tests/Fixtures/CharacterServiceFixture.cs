using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures
{
    public class CharacterServiceFixture
    {
        public CharacterServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            CharacterRepository = fixture.Freeze<IRepository<Character>>();
            Logger = fixture.Freeze<ILogger<CharacterService>>();

            CharacterService = new CharacterService(
                CharacterRepository,
                Logger);

            Id = 1;
            Name = "StringPlaceholder";
            Character = GetCharacter();
            Characters = GetCharacters();
            PaginatedList = GetPaginatedList();
        }

        public ICharacterService CharacterService { get; }
        public IRepository<Character> CharacterRepository { get; set; }
        public ILogger<CharacterService> Logger { get; set; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public List<Character> Characters { get; }
        public PaginatedList<Character> PaginatedList { get; }

        private Spell GetSpell(int id)
        {
            return new Spell()
            {
                Id = id,
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private Weapon GetWeapon(int id)
        {
            return new Weapon()
            {
                Id = id,
                Name = Name,
                Type = WeaponType.Sword,
                Damage = 20
            };
        }

        private CharacterWeapon GetCharacterWeapon(int characterId, int weaponId)
        {
            return new CharacterWeapon()
            {
                CharacterId = characterId,
                Character = Character,
                WeaponId = weaponId,
                Weapon = GetWeapon(weaponId)
            };
        }

        private CharacterSpell GetCharacterSpell(int characterId, int spellId)
        {
            return new CharacterSpell()
            {
                CharacterId = characterId,
                Character = Character,
                SpellId = spellId,
                Spell = GetSpell(spellId)
            };
        }

        private ICollection<CharacterWeapon> GetCharacterWeapons()
        {
            return new List<CharacterWeapon>()
            {
                GetCharacterWeapon(Id, Id),
                GetCharacterWeapon(Id, 2)
            };
        }

        private ICollection<CharacterSpell> GetCharacterSpells()
        {
            return new List<CharacterSpell>()
            {
                GetCharacterSpell(Id, Id),
                GetCharacterSpell(Id, 2)
            };
        }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = 1,
                Name = Name,
                Race = CharacterRace.Human,
                Health = 100,
                PlayerId = Id,
                CharacterWeapons = GetCharacterWeapons(),
                CharacterSpells = GetCharacterSpells()
            };
        }

        private List<Character> GetCharacters()
        {
            return new List<Character>()
            {
                Character,
                Character
            };
        }

        private PaginatedList<Character> GetPaginatedList()
        {
            return new PaginatedList<Character>(Characters, 6, 1, 5);
        }
    }
}

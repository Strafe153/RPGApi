using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests.Fixtures
{
    public class CharacterServiceFixture
    {
        public CharacterServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockCharacterRepository = fixture.Freeze<Mock<IRepository<Character>>>();
            MockLogger = fixture.Freeze<Mock<ILogger<CharacterService>>>();

            MockCharacterService = new CharacterService(
                MockCharacterRepository.Object,
                MockLogger.Object);

            Id = 1;
            Name = "StringPlaceholder";
            Character = GetCharacter();
            Characters = GetCharacters();
            PagedList = GetPagedList();
        }

        public ICharacterService MockCharacterService { get; }
        public Mock<IRepository<Character>> MockCharacterRepository { get; set; }
        public Mock<ILogger<CharacterService>> MockLogger { get; set; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public List<Character> Characters { get; }
        public PagedList<Character> PagedList { get; }

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

        private PagedList<Character> GetPagedList()
        {
            return new PagedList<Character>(Characters, 6, 1, 5);
        }
    }
}

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
    public class SpellServiceFixture
    {
        public SpellServiceFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockSpellRepository = fixture.Freeze<Mock<IRepository<Spell>>>();
            MockLogger = fixture.Freeze<Mock<ILogger<SpellService>>>();

            MockSpellService = new SpellService(
                MockSpellRepository.Object,
                MockLogger.Object);

            Id = 1;
            Name = "StringPlaceholder";
            Spell = GetSpell(Id);
            Character = GetCharacter();
            PaginatedList = GetPaginatedList();
        }

        public IItemService<Spell> MockSpellService { get; }
        public Mock<IRepository<Spell>> MockSpellRepository { get; }
        public Mock<ILogger<SpellService>> MockLogger { get; set; }

        public int Id { get; }
        public string? Name { get; }
        public Spell Spell { get; }
        public Character Character { get; }
        public PaginatedList<Spell> PaginatedList { get; }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = Id,
                Name = Name,
                Race = CharacterRace.Human,
                Health = 100,
                CharacterSpells = GetCharacterSpells()
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

        private ICollection<CharacterSpell> GetCharacterSpells()
        {
            return new List<CharacterSpell>()
            {
                GetCharacterSpell(Id, 2),
                GetCharacterSpell(Id, 3)
            };
        }

        private Spell GetSpell(int id)
        {
            return new Spell()
            {
                Id = id,
                Name = Name,
                Damage = 5,
                Type = SpellType.Fire
            };
        }

        private List<Spell> GetSpells()
        {
            return new List<Spell>()
            {
                Spell,
                Spell
            };
        }

        private PaginatedList<Spell> GetPaginatedList()
        {
            return new PaginatedList<Spell>(GetSpells(), 6, 1, 5);
        }
    }
}

using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class CharacterServiceFixture
{
    public CharacterServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        CharacterId = Random.Shared.Next();
        WeaponId = Random.Shared.Next();
        SpellId = Random.Shared.Next();
        PageNumber = Random.Shared.Next(1, 25);
        PageSize = Random.Shared.Next(1, 100);
        CharactersCount = Random.Shared.Next(1, 20);

        var characterFaker = new Faker<Character>();

        var weaponFaker = new Faker<Weapon>()
            .RuleFor(w => w.Id, WeaponId)
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
            .RuleFor(w => w.Type, f => (WeaponType)f.Random.Int(Enum.GetValues(typeof(WeaponType)).Length));

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, SpellId)
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (SpellType)f.Random.Int(Enum.GetValues(typeof(SpellType)).Length));

        var characterWeaponFaker = new Faker<CharacterWeapon>()
            .RuleFor(cw => cw.CharacterId, CharacterId)
            .RuleFor(cw => cw.WeaponId, WeaponId)
            .RuleFor(cw => cw.Character, characterFaker)
            .RuleFor(cw => cw.Weapon, weaponFaker);

        var characterSpellFaker = new Faker<CharacterSpell>()
            .RuleFor(cw => cw.CharacterId, CharacterId)
            .RuleFor(cw => cw.SpellId, SpellId)
            .RuleFor(cw => cw.Character, characterFaker)
            .RuleFor(cw => cw.Spell, spellFaker);

        characterFaker
            .RuleFor(c => c.Id, CharacterId)
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length))
            .RuleFor(c => c.CharacterWeapons, new[] { characterWeaponFaker.Generate() })
            .RuleFor(c => c.CharacterSpells, new[] {characterSpellFaker.Generate() });

        var paginatedListFaker = new Faker<PaginatedList<Character>>()
            .CustomInstantiator(f => new(
                characterFaker.Generate(CharactersCount),
                CharactersCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        CharacterRepository = fixture.Freeze<IRepository<Character>>();
        CacheService = fixture.Freeze<ICacheService>();
        Logger = fixture.Freeze<ILogger<CharacterService>>();

        CharacterService = new CharacterService(
            CharacterRepository,
            CacheService,
            Logger);

        Character = characterFaker.Generate();
        Characters = characterFaker.Generate(CharactersCount);
        PaginatedList = paginatedListFaker.Generate();
    }
    private int CharactersCount { get; }

    public ICharacterService CharacterService { get; }
    public IRepository<Character> CharacterRepository { get; set; }
    public ICacheService CacheService { get; }
    public ILogger<CharacterService> Logger { get; set; }

    public int CharacterId { get; }
    public int WeaponId { get; }
    public int SpellId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public Character Character { get; }
    public List<Character> Characters { get; }
    public PaginatedList<Character> PaginatedList { get; }
}

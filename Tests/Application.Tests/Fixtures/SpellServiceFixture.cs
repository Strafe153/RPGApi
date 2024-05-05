using Application.Services;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class SpellServiceFixture
{
    public SpellServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        PageNumber = Random.Shared.Next(1, 25);
        PageSize = Random.Shared.Next(1, 100);
        SpellsCount = Random.Shared.Next(1, 20);

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, Id)
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (SpellType)f.Random.Int(Enum.GetValues(typeof(SpellType)).Length));

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var paginatedListFaker = new Faker<PaginatedList<Spell>>()
            .CustomInstantiator(f => new(
                spellFaker.Generate(SpellsCount),
                SpellsCount,
                PageNumber,
                PageSize));

        SpellRepository = fixture.Freeze<IItemRepository<Spell>>();
        Logger = fixture.Freeze<ILogger<SpellService>>();

        SpellService = new SpellService(SpellRepository, Logger);

        Spell = spellFaker.Generate();
        Character = characterFaker.Generate();
        Spells = spellFaker.Generate(SpellsCount);
        PaginatedList = paginatedListFaker.Generate();
    }

    private int SpellsCount { get; }

    public IItemService<Spell> SpellService { get; }
    public IItemRepository<Spell> SpellRepository { get; }
    public ILogger<SpellService> Logger { get; set; }

    public int Id { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public Spell Spell { get; }
    public Character Character { get; }
    public List<Spell> Spells { get; }
    public PaginatedList<Spell> PaginatedList { get; }
}

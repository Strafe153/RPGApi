using Application.Dtos.SpellDtos;
using Application.Services.Abstractions;
using Application.Services.Implementations;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Tests.Fixtures;

public class SpellServiceFixture
{
    public SpellServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        SpellsCount = Random.Shared.Next(1, 20);
        PageParameters = new()
        {
            PageNumber = Random.Shared.Next(1, 25),
            PageSize = Random.Shared.Next(1, 100)
        };

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, Id)
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => f.PickRandom<SpellType>());

        var spellCreateDtoFaker = new Faker<SpellCreateDto>()
            .CustomInstantiator(f => new(
                f.Commerce.ProductName(),
                f.PickRandom<SpellType>(),
                f.Random.Int(1, 100)));

        var spellUpdateDtoFaker = new Faker<SpellUpdateDto>()
            .CustomInstantiator(f => new(
                f.Commerce.ProductName(),
                f.PickRandom<SpellType>(),
                f.Random.Int(1, 100)));

        var pagedListFaker = new Faker<PagedList<Spell>>()
            .CustomInstantiator(f => new(
                spellFaker.Generate(SpellsCount),
                SpellsCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        SpellsRepository = fixture.Freeze<IItemRepository<Spell>>();
        Logger = fixture.Freeze<ILogger<SpellsService>>();

        SpellsService = new SpellsService(SpellsRepository, Logger);

        Spell = spellFaker.Generate();
        SpellCreateDto = spellCreateDtoFaker.Generate();
        SpellUpdateDto = spellUpdateDtoFaker.Generate();
        Spells = spellFaker.Generate(SpellsCount);
        PagedList = pagedListFaker.Generate();
        PatchDocument = new();
    }

    private int SpellsCount { get; }

    public ISpellsService SpellsService { get; }
    public IItemRepository<Spell> SpellsRepository { get; }
    public ILogger<SpellsService> Logger { get; set; }

    public int Id { get; }
    public PageParameters PageParameters { get; }
    public Spell Spell { get; }
    public SpellCreateDto SpellCreateDto { get; }
    public SpellUpdateDto SpellUpdateDto { get; }
    public List<Spell> Spells { get; }
    public PagedList<Spell> PagedList { get; }
    public JsonPatchDocument<SpellUpdateDto> PatchDocument { get; }
    public CancellationToken CancellationToken { get; }
}

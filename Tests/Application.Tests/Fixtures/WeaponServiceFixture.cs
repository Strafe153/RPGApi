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

public class WeaponServiceFixture
{
    public WeaponServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        PageNumber = Random.Shared.Next(1, 25);
        PageSize = Random.Shared.Next(1, 100);
        WeaponsCount = Random.Shared.Next(1, 20);
        
        var weaponFaker = new Faker<Weapon>()
            .RuleFor(w => w.Id, Id)
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
            .RuleFor(w => w.Type, f => (WeaponType)f.Random.Int(Enum.GetValues(typeof(WeaponType)).Length));

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var paginatedListFaker = new Faker<PaginatedList<Weapon>>()
            .CustomInstantiator(f => new(
                weaponFaker.Generate(WeaponsCount),
                WeaponsCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        WeaponRepository = fixture.Freeze<IItemRepository<Weapon>>();
        Logger = fixture.Freeze<ILogger<WeaponService>>();

        WeaponService = new WeaponService(
            WeaponRepository,
            Logger);

        Weapon = weaponFaker.Generate();
        Character = characterFaker.Generate();
        Weapons = weaponFaker.Generate(WeaponsCount);
        PaginatedList = paginatedListFaker.Generate();
    }

    private int WeaponsCount { get; }

    public IItemService<Weapon> WeaponService { get; }
    public IItemRepository<Weapon> WeaponRepository { get; }
    public ILogger<WeaponService> Logger { get; set; }

    public int Id { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public Weapon Weapon { get; }
    public Character Character { get; }
    public List<Weapon> Weapons { get; }
    public PaginatedList<Weapon> PaginatedList { get; }
}

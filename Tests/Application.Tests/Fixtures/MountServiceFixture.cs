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

public class MountServiceFixture
{
    public MountServiceFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        PageNumber = Random.Shared.Next(1, 25);
        PageSize = Random.Shared.Next(1, 100);
        MountsCount = Random.Shared.Next(1, 20);

        var mountFaker = new Faker<Mount>()
           .RuleFor(m => m.Id, Id)
           .RuleFor(m => m.Name, f => f.Name.FirstName())
           .RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
           .RuleFor(m => m.Type, f => (MountType)f.Random.Int(Enum.GetValues(typeof(MountType)).Length));

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, f => f.Random.Int())
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var paginatedListFaker = new Faker<PaginatedList<Mount>>()
            .CustomInstantiator(f => new(
                mountFaker.Generate(MountsCount),
                MountsCount,
                PageNumber,
                PageSize));

        MountRepository = fixture.Freeze<IItemRepository<Mount>>();
        CacheService = fixture.Freeze<ICacheService>();
        Logger = fixture.Freeze<ILogger<MountService>>();

        MountService = new MountService(
            MountRepository,
            CacheService,
            Logger);

        Mount = mountFaker.Generate();
        Character = characterFaker.Generate();
        Mounts = mountFaker.Generate(MountsCount);
        PaginatedList = paginatedListFaker.Generate();
    }

    private int MountsCount { get; }

    public IItemService<Mount> MountService { get; }
    public IItemRepository<Mount> MountRepository { get; }
    public ICacheService CacheService { get; }
    public ILogger<MountService> Logger { get; }

    public int Id { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public Mount Mount { get; }
    public Character Character { get; }
    public List<Mount> Mounts { get; }
    public PaginatedList<Mount> PaginatedList { get; }
}

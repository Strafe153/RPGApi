using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using WebApi.Controllers.V1;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.WeaponMappers;

namespace WebApi.Tests.V1.Fixtures;

public class WeaponsControllerFixture
{
    public WeaponsControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        WeaponsCount = Random.Shared.Next(1, 20);

        var weaponFaker = new Faker<Weapon>()
            .RuleFor(w => w.Id, f => f.Random.Int())
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
            .RuleFor(w => w.Type, f => (WeaponType)f.Random.Int(Enum.GetValues(typeof(WeaponType)).Length));

        var weaponBaseDtoFaker = new Faker<WeaponBaseDto>()
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (WeaponType)f.Random.Int(Enum.GetValues(typeof(WeaponType)).Length));

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, Id)
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var hitDtoFaker = new Faker<HitDto>()
            .RuleFor(h => h.DealerId, f => f.Random.Int())
            .RuleFor(h => h.ReceiverId, f => f.Random.Int())
            .RuleFor(h => h.ItemId, Id);

        var pageParametersFaker = new Faker<PageParameters>()
            .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
            .RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

        var paginatedListFaker = new Faker<PaginatedList<Weapon>>()
            .CustomInstantiator(f => new(
                weaponFaker.Generate(WeaponsCount),
                WeaponsCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        WeaponService = fixture.Freeze<IItemService<Weapon>>();
        CharacterService = fixture.Freeze<ICharacterService>();
        PlayerService = fixture.Freeze<IPlayerService>();
        ReadMapper = new WeaponReadMapper();
        PaginatedMapper = new WeaponPaginatedMapper(ReadMapper);
        CreateMapper = new WeaponCreateMapper();
        UpdateMapper = new WeaponUpdateMapper();

        WeaponsController = new WeaponsController(
            WeaponService,
            CharacterService,
            PlayerService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Character = characterFaker.Generate();
        Weapon = weaponFaker.Generate();
        WeaponBaseDto = weaponBaseDtoFaker.Generate();
        HitDto = hitDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
        PatchDocument = new JsonPatchDocument<WeaponBaseDto>();
    }

    public WeaponsController WeaponsController { get; }
    public IItemService<Weapon> WeaponService { get; }
    public ICharacterService CharacterService { get; }
    public IPlayerService PlayerService { get; }
    public IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>> PaginatedMapper { get; }
    public IMapper<Weapon, WeaponReadDto> ReadMapper { get; }
    public IMapper<WeaponBaseDto, Weapon> CreateMapper { get; }
    public IUpdateMapper<WeaponBaseDto, Weapon> UpdateMapper { get; }

    public int Id { get; }
    public int WeaponsCount { get; }
    public Character Character { get; }
    public Weapon Weapon { get; }
    public WeaponBaseDto WeaponBaseDto { get; }
    public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Weapon> PaginatedList { get; }
    public JsonPatchDocument<WeaponBaseDto> PatchDocument { get; }
    public CancellationToken CancellationToken { get; }

    public void MockObjectModelValidator(ControllerBase controller)
    {
        var objectValidator = Substitute.For<IObjectModelValidator>();

        objectValidator.Validate(
            Arg.Any<ActionContext>(),
            Arg.Any<ValidationStateDictionary>(),
            Arg.Any<string>(),
            Arg.Any<object>());

        controller.ObjectValidator = objectValidator;
    }

    public ControllerContext MockControllerContext() =>
        new ControllerContext(
            new ActionContext(
                new DefaultHttpContext() { TraceIdentifier = "trace" },
                new RouteData(),
                new ControllerActionDescriptor()));

    public void MockModelError(ControllerBase controller)
    {
        var context = MockControllerContext();

        context.ModelState.AddModelError("key", "error");
        controller.ControllerContext = context;
    }
}

using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Services;
using Domain.Shared;
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
            .RuleFor(w => w.Type, f => f.PickRandom<WeaponType>());

        var weaponCreateDtoFaker = new Faker<WeaponCreateDto>()
            .CustomInstantiator(f => new(
                f.Commerce.ProductName(),
                f.PickRandom<WeaponType>(),
                f.Random.Int(1, 100)));

		var weaponUpdateDtoFaker = new Faker<WeaponUpdateDto>()
			.CustomInstantiator(f => new(
				f.Commerce.ProductName(),
				f.PickRandom<WeaponType>(),
				f.Random.Int(1, 100)));

		var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, Id)
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => f.PickRandom<CharacterRace>());

        var hitDtoFaker = new Faker<HitDto>()
            .CustomInstantiator(f => new(f.Random.Int(), f.Random.Int(), Id));

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

        WeaponsController = new(
            WeaponService,
            CharacterService,
            PlayerService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Character = characterFaker.Generate();
        Weapon = weaponFaker.Generate();
        WeaponCreateDto = weaponCreateDtoFaker.Generate();
		WeaponUpdateDto = weaponUpdateDtoFaker.Generate();
		HitDto = hitDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
        PatchDocument = new JsonPatchDocument<WeaponUpdateDto>();
    }

    public WeaponsController WeaponsController { get; }
    public IItemService<Weapon> WeaponService { get; }
    public ICharacterService CharacterService { get; }
    public IPlayerService PlayerService { get; }
    public IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>> PaginatedMapper { get; }
    public IMapper<Weapon, WeaponReadDto> ReadMapper { get; }
    public IMapper<WeaponCreateDto, Weapon> CreateMapper { get; }
    public IUpdateMapper<WeaponUpdateDto, Weapon> UpdateMapper { get; }

    public int Id { get; }
    public int WeaponsCount { get; }
    public Character Character { get; }
    public Weapon Weapon { get; }
    public WeaponCreateDto WeaponCreateDto { get; }
	public WeaponUpdateDto WeaponUpdateDto { get; }
	public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Weapon> PaginatedList { get; }
    public JsonPatchDocument<WeaponUpdateDto> PatchDocument { get; }
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

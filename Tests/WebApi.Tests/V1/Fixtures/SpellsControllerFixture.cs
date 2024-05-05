using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos;
using Domain.Dtos.SpellDtos;
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
using WebApi.Mappers.SpellMappers;

namespace WebApi.Tests.V1.Fixtures;

public class SpellsControllerFixture
{
    public SpellsControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        SpellsCount = Random.Shared.Next(1, 20);

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, Id)
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => f.PickRandom<CharacterRace>());

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, f => f.Random.Int())
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

		var hitDtoFaker = new Faker<HitDto>()
            .CustomInstantiator(f => new(f.Random.Int(), f.Random.Int(), Id));

        var pageParametersFaker = new Faker<PageParameters>()
            .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
            .RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

        var paginatedListFaker = new Faker<PaginatedList<Spell>>()
            .CustomInstantiator(f => new(
                spellFaker.Generate(SpellsCount),
                SpellsCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        SpellService = fixture.Freeze<IItemService<Spell>>();
        CharacterService = fixture.Freeze<ICharacterService>();
        PlayerService = fixture.Freeze<IPlayerService>();
        ReadMapper = new SpellReadMapper();
        PaginatedMapper = new SpellPaginatedMapper(ReadMapper);
        CreateMapper = new SpellCreateMapper();
        UpdateMapper = new SpellUpdateMapper();

        SpellsController = new(
            SpellService,
            CharacterService,
            PlayerService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Character = characterFaker.Generate();
        Spell = spellFaker.Generate();
        SpellCreateDto = spellCreateDtoFaker.Generate();
		SpellUpdateDto = spellUpdateDtoFaker.Generate();
		HitDto = hitDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
        PatchDocument = new JsonPatchDocument<SpellUpdateDto>();
    }

    public SpellsController SpellsController { get; }
    public IItemService<Spell> SpellService { get; }
    public ICharacterService CharacterService { get; }
    public IPlayerService PlayerService { get; }
    public IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>> PaginatedMapper { get; }
    public IMapper<Spell, SpellReadDto> ReadMapper { get; }
    public IMapper<SpellCreateDto, Spell> CreateMapper { get; }
    public IUpdateMapper<SpellUpdateDto, Spell> UpdateMapper { get; }

    public int Id { get; }
    public int SpellsCount { get; }
    public Character Character { get; }
    public Spell Spell { get; }
    public SpellCreateDto SpellCreateDto { get; }
	public SpellUpdateDto SpellUpdateDto { get; }
	public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Spell> PaginatedList { get; }
    public JsonPatchDocument<SpellUpdateDto> PatchDocument { get; }
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

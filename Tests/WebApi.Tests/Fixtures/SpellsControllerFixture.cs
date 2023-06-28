using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Dtos;
using Core.Dtos.SpellDtos;
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
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;
using WebApi.Mappers.SpellMappers;

namespace WebApi.Tests.Fixtures;

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
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, f => f.Random.Int())
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (SpellType)f.Random.Int(Enum.GetValues(typeof(SpellType)).Length));

        var spellBaseDtoFaker = new Faker<SpellBaseDto>()
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (SpellType)f.Random.Int(Enum.GetValues(typeof(SpellType)).Length));

        var hitDtoFaker = new Faker<HitDto>()
            .RuleFor(h => h.DealerId, f => f.Random.Int())
            .RuleFor(h => h.ReceiverId, f => f.Random.Int())
            .RuleFor(h => h.ItemId, Id);

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

        SpellsController = new SpellsController(
            SpellService,
            CharacterService,
            PlayerService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Character = characterFaker.Generate();
        Spell = spellFaker.Generate();
        SpellBaseDto = spellBaseDtoFaker.Generate();
        HitDto = hitDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
        PatchDocument = new JsonPatchDocument<SpellBaseDto>(); ;
    }

    public SpellsController SpellsController { get; }
    public IItemService<Spell> SpellService { get; }
    public ICharacterService CharacterService { get; }
    public IPlayerService PlayerService { get; }
    public IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>> PaginatedMapper { get; }
    public IMapper<Spell, SpellReadDto> ReadMapper { get; }
    public IMapper<SpellBaseDto, Spell> CreateMapper { get; }
    public IUpdateMapper<SpellBaseDto, Spell> UpdateMapper { get; }

    public int Id { get; }
    public int SpellsCount { get; }
    public Character Character { get; }
    public Spell Spell { get; }
    public SpellBaseDto SpellBaseDto { get; }
    public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Spell> PaginatedList { get; }
    public JsonPatchDocument<SpellBaseDto> PatchDocument { get; }
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

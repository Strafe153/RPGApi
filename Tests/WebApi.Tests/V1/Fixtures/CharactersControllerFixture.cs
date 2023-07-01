using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Core.Dtos;
using Core.Dtos.CharacterDtos;
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
using WebApi.Mappers.CharacterMappers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.V1.Fixtures;

public class CharactersControllerFixture
{
    public CharactersControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        CharactersCount = Random.Shared.Next(1, 20);

        var weaponFaker = new Faker<Weapon>()
            .RuleFor(w => w.Id, f => f.Random.Int())
            .RuleFor(w => w.Name, f => f.Commerce.ProductName())
            .RuleFor(w => w.Damage, f => f.Random.Int(1, 100))
            .RuleFor(w => w.Type, f => (WeaponType)f.Random.Int(Enum.GetValues(typeof(WeaponType)).Length));

        var spellFaker = new Faker<Spell>()
            .RuleFor(s => s.Id, f => f.Random.Int())
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Damage, f => f.Random.Int(1, 100))
            .RuleFor(s => s.Type, f => (SpellType)f.Random.Int(Enum.GetValues(typeof(SpellType)).Length));

        var mountFaker = new Faker<Mount>()
            .RuleFor(m => m.Id, f => f.Random.Int())
            .RuleFor(m => m.Name, f => f.Name.FirstName())
            .RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
            .RuleFor(m => m.Type, f => (MountType)f.Random.Int(Enum.GetValues(typeof(MountType)).Length));

        var characterFaker = new Faker<Character>()
            .RuleFor(c => c.Id, Id)
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Health, f => f.Random.Int(1, 100))
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var characterCreateDtoFaker = new Faker<CharacterCreateDto>()
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length))
            .RuleFor(c => c.PlayerId, f => f.Random.Int());

        var characterUpdateDtoFaker = new Faker<CharacterBaseDto>()
            .RuleFor(c => c.Name, f => f.Internet.UserName())
            .RuleFor(c => c.Race, f => (CharacterRace)f.Random.Int(Enum.GetValues(typeof(CharacterRace)).Length));

        var addRemoveItemDtoFaker = new Faker<AddRemoveItemDto>()
            .RuleFor(c => c.CharacterId, Id)
            .RuleFor(c => c.ItemId, f => f.Random.Int());

        var pageParametersFaker = new Faker<PageParameters>()
            .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
            .RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

        var paginatedListFaker = new Faker<PaginatedList<Character>>()
            .CustomInstantiator(f => new(
                characterFaker.Generate(CharactersCount),
                CharactersCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        CharacterService = fixture.Freeze<ICharacterService>();
        PlayerService = fixture.Freeze<IPlayerService>();
        WeaponService = fixture.Freeze<IItemService<Weapon>>();
        SpellService = fixture.Freeze<IItemService<Spell>>();
        MountService = fixture.Freeze<IItemService<Mount>>();
        ReadMapper = new CharacterReadMapper();
        PaginatedMapper = new CharacterPaginatedMapper(ReadMapper);
        CreateMapper = new CharacterCreateMapper();
        UpdateMapper = new CharacterUpdateMapper();

        CharactersController = new CharactersController(
            CharacterService,
            PlayerService,
            WeaponService,
            SpellService,
            MountService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Weapon = weaponFaker.Generate();
        Spell = spellFaker.Generate();
        Mount = mountFaker.Generate();
        Character = characterFaker.Generate();
        CharacterCreateDto = characterCreateDtoFaker.Generate();
        CharacterUpdateDto = characterUpdateDtoFaker.Generate();
        PatchDocument = new JsonPatchDocument<CharacterBaseDto>();
        ItemDto = addRemoveItemDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
    }

    public CharactersController CharactersController { get; }
    public ICharacterService CharacterService { get; }
    public IPlayerService PlayerService { get; }
    public IItemService<Weapon> WeaponService { get; }
    public IItemService<Spell> SpellService { get; }
    public IItemService<Mount> MountService { get; }
    public IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>> PaginatedMapper { get; }
    public IMapper<Character, CharacterReadDto> ReadMapper { get; }
    public IMapper<CharacterCreateDto, Character> CreateMapper { get; }
    public IUpdateMapper<CharacterBaseDto, Character> UpdateMapper { get; }

    public int Id { get; }
    public int CharactersCount { get; }
    public Weapon Weapon { get; }
    public Spell Spell { get; }
    public Mount Mount { get; }
    public Character Character { get; }
    public CharacterCreateDto CharacterCreateDto { get; }
    public CharacterBaseDto CharacterUpdateDto { get; }
    public JsonPatchDocument<CharacterBaseDto> PatchDocument { get; }
    public AddRemoveItemDto ItemDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Character> PaginatedList { get; }
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
                new DefaultHttpContext()
                {
                    TraceIdentifier = "trace"
                },
                new RouteData(),
                new ControllerActionDescriptor()));

    public void MockModelError(ControllerBase controller)
    {
        var context = MockControllerContext();

        context.ModelState.AddModelError("key", "error");
        controller.ControllerContext = context;
    }
}

using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures;

public class WeaponsControllerFixture
{
    public WeaponsControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        WeaponService = fixture.Freeze<IItemService<Weapon>>();
        CharacterService = fixture.Freeze<ICharacterService>();
        PlayerService = fixture.Freeze<IPlayerService>();
        PaginatedMapper = fixture.Freeze<IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>>();
        ReadMapper = fixture.Freeze<IMapper<Weapon, WeaponReadDto>>();
        CreateMapper = fixture.Freeze<IMapper<WeaponBaseDto, Weapon>>();
        UpdateMapper = fixture.Freeze<IUpdateMapper<WeaponBaseDto, Weapon>>();

        WeaponsController = new WeaponsController(
            WeaponService,
            CharacterService,
            PlayerService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Id = 1;
        Name = "Name";
        Character = GetCharacter();
        Weapon = GetWeapon();
        WeaponReadDto = GetWeaponReadDto();
        WeaponBaseDto = GetWeaponBaseDto();
        HitDto = GetHitDto();
        PageParameters = GetPageParameters();
        PaginatedList = GetPaginatedList();
        PageDto = GetPageDto();
        PatchDocument = GetPatchDocument();
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
    public string? Name { get; }
    public Character Character { get; }
    public Weapon Weapon { get; }
    public WeaponReadDto WeaponReadDto { get; }
    public WeaponBaseDto WeaponBaseDto { get; }
    public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Weapon> PaginatedList { get; }
    public PageDto<WeaponReadDto> PageDto { get; }
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

    public ControllerContext MockControllerContext()
    {
        var context = new ControllerContext(
            new ActionContext(
                new DefaultHttpContext() { TraceIdentifier = "trace" },
                new RouteData(),
                new ControllerActionDescriptor()));

        return context;
    }

    public void MockModelError(ControllerBase controller)
    {
        var context = MockControllerContext();

        context.ModelState.AddModelError("key", "error");
        controller.ControllerContext = context;
    }

    private Character GetCharacter()
    {
        return new Character()
        {
            Id = Id,
            Name = Name,
            Race = CharacterRace.Human,
            Health = 100,
            PlayerId = Id
        };
    }

    private Weapon GetWeapon()
    {
        return new Weapon()
        {
            Id = Id,
            Name = Name,
            Type = WeaponType.Sword,
            Damage = 20
        };
    }

    private List<Weapon> GetWeapons()
    {
        return new List<Weapon>()
        {
            Weapon,
            Weapon
        };
    }

    private PageParameters GetPageParameters()
    {
        return new PageParameters()
        {
            PageNumber = 1,
            PageSize = 5
        };
    }

    private PaginatedList<Weapon> GetPaginatedList()
    {
        return new PaginatedList<Weapon>(GetWeapons(), 6, 1, 5);
    }

    private WeaponBaseDto GetWeaponBaseDto()
    {
        return new WeaponBaseDto()
        {
            Name = Name,
            Type = WeaponType.Sword,
            Damage = 20
        };
    }

    private WeaponReadDto GetWeaponReadDto()
    {
        return new WeaponReadDto()
        {
            Id = Id,
            Name = Name,
            Type = WeaponType.Sword,
            Damage = 20
        };
    }

    private List<WeaponReadDto> GetWeaponReadDtos()
    {
        return new List<WeaponReadDto>()
        {
            WeaponReadDto,
            WeaponReadDto
        };
    }

    private HitDto GetHitDto()
    {
        return new HitDto()
        {
            DealerId = Id,
            ItemId = Id,
            ReceiverId = Id
        };
    }

    private PageDto<WeaponReadDto> GetPageDto()
    {
        return new PageDto<WeaponReadDto>()
        {
            CurrentPage = 1,
            TotalPages = 2,
            PageSize = 5,
            TotalItems = 6,
            HasPrevious = false,
            HasNext = true,
            Entities = GetWeaponReadDtos()
        };
    }

    private JsonPatchDocument<WeaponBaseDto> GetPatchDocument()
    {
        return new JsonPatchDocument<WeaponBaseDto>();
    }
}

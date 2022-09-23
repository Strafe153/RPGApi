using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Dtos;
using Core.Dtos.MountDtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures;

public class MountsControllerFixture
{
    public MountsControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        MountService = fixture.Freeze<IItemService<Mount>>();
        PaginatedMapper = fixture.Freeze<IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>>();
        ReadMapper = fixture.Freeze<IMapper<Mount, MountReadDto>>();
        CreateMapper = fixture.Freeze<IMapper<MountBaseDto, Mount>>();
        UpdateMapper = fixture.Freeze<IUpdateMapper<MountBaseDto, Mount>>();

        MountsController = new(
            MountService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Id = 1;
        Name = "Name";
        Character = GetCharacter();
        Mount = GetMount();
        MountReadDto = GetMountReadDto();
        MountBaseDto = GetMountBaseDto();
        HitDto = GetHitDto();
        PageParameters = GetPageParameters();
        PaginatedList = GetPaginatedList();
        PageDto = GetPageViewModel();
        PatchDocument = GetPatchDocument();
    }

    public MountsController MountsController { get; }
    public IItemService<Mount> MountService { get; }
    public IMapper<PaginatedList<Mount>, PageDto<MountReadDto>> PaginatedMapper { get; }
    public IMapper<Mount, MountReadDto> ReadMapper { get; }
    public IMapper<MountBaseDto, Mount> CreateMapper { get; }
    public IUpdateMapper<MountBaseDto, Mount> UpdateMapper { get; }

    public int Id { get; }
    public string? Name { get; }
    public Character Character { get; }
    public Mount Mount { get; }
    public MountReadDto MountReadDto { get; }
    public MountBaseDto MountBaseDto { get; }
    public HitDto HitDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Mount> PaginatedList { get; }
    public PageDto<MountReadDto> PageDto { get; }
    public JsonPatchDocument<MountBaseDto> PatchDocument { get; }

    public void MockControllerBaseUser()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        MountsController.ControllerContext = new ControllerContext();
        MountsController.ControllerContext.HttpContext = new DefaultHttpContext()
        {
            User = user
        };
    }

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

    private Mount GetMount()
    {
        return new Mount()
        {
            Id = Id,
            Name = Name,
            Type = MountType.Horse,
            Speed = 10
        };
    }

    private List<Mount> GetMounts()
    {
        return new List<Mount>()
        {
            Mount,
            Mount
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

    private PaginatedList<Mount> GetPaginatedList()
    {
        return new PaginatedList<Mount>(GetMounts(), 6, 1, 5);
    }

    private MountBaseDto GetMountBaseDto()
    {
        return new MountBaseDto()
        {
            Name = Name,
            Type = MountType.Horse,
            Speed = 10
        };
    }

    private MountReadDto GetMountReadDto()
    {
        return new MountReadDto()
        {
            Id = Id,
            Name = Name,
            Type = MountType.Horse,
            Speed = 10
        };
    }

    private List<MountReadDto> GetMountReadDtos()
    {
        return new List<MountReadDto>()
        {
            MountReadDto,
            MountReadDto
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

    private PageDto<MountReadDto> GetPageViewModel()
    {
        return new PageDto<MountReadDto>()
        {
            CurrentPage = 1,
            TotalPages = 2,
            PageSize = 5,
            TotalItems = 6,
            HasPrevious = false,
            HasNext = true,
            Entities = GetMountReadDtos()
        };
    }

    private JsonPatchDocument<MountBaseDto> GetPatchDocument()
    {
        return new JsonPatchDocument<MountBaseDto>();
    }
}

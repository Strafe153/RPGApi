using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Bogus;
using Domain.Dtos;
using Domain.Dtos.MountDtos;
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
using WebApi.Mappers.MountMappers;

namespace WebApi.Tests.V1.Fixtures;

public class MountsControllerFixture
{
    public MountsControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Id = Random.Shared.Next();
        MountsCount = Random.Shared.Next(1, 20);

        var mountFaker = new Faker<Mount>()
            .RuleFor(m => m.Id, f => f.Random.Int())
            .RuleFor(m => m.Name, f => f.Name.FirstName())
            .RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
            .RuleFor(m => m.Type, f => (MountType)f.Random.Int(Enum.GetValues(typeof(MountType)).Length));

        var mountBaseDtoFaker = new Faker<MountBaseDto>()
            .RuleFor(m => m.Name, f => f.Name.FirstName())
            .RuleFor(m => m.Speed, f => f.Random.Int(1, 100))
            .RuleFor(m => m.Type, f => (MountType)f.Random.Int(Enum.GetValues(typeof(MountType)).Length));

        var pageParametersFaker = new Faker<PageParameters>()
            .RuleFor(p => p.PageNumber, f => f.Random.Int(1, 100))
            .RuleFor(p => p.PageSize, f => f.Random.Int(1, 100));

        var paginatedListFaker = new Faker<PaginatedList<Mount>>()
            .CustomInstantiator(f => new(
                mountFaker.Generate(MountsCount),
                MountsCount,
                f.Random.Int(1, 2),
                f.Random.Int(1, 2)));

        MountService = fixture.Freeze<IItemService<Mount>>();
        ReadMapper = new MountReadMapper();
        PaginatedMapper = new MountPaginatedMapper(ReadMapper);
        CreateMapper = new MountCreateMapper();
        UpdateMapper = new MountUpdateMapper();

        MountsController = new MountsController(
            MountService,
            PaginatedMapper,
            ReadMapper,
            CreateMapper,
            UpdateMapper);

        Mount = mountFaker.Generate();
        MountBaseDto = mountBaseDtoFaker.Generate();
        PageParameters = pageParametersFaker.Generate();
        PaginatedList = paginatedListFaker.Generate();
        PatchDocument = new JsonPatchDocument<MountBaseDto>();
    }

    public MountsController MountsController { get; }
    public IItemService<Mount> MountService { get; }
    public IMapper<PaginatedList<Mount>, PageDto<MountReadDto>> PaginatedMapper { get; }
    public IMapper<Mount, MountReadDto> ReadMapper { get; }
    public IMapper<MountBaseDto, Mount> CreateMapper { get; }
    public IUpdateMapper<MountBaseDto, Mount> UpdateMapper { get; }

    public int Id { get; }
    public int MountsCount { get; }
    public Mount Mount { get; }
    public MountBaseDto MountBaseDto { get; }
    public PageParameters PageParameters { get; }
    public PaginatedList<Mount> PaginatedList { get; }
    public JsonPatchDocument<MountBaseDto> PatchDocument { get; }
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

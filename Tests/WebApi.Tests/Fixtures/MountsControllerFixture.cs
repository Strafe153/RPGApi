using AutoFixture;
using AutoFixture.AutoMoq;
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
using Moq;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures
{
    public class MountsControllerFixture
    {
        public MountsControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockMountService = fixture.Freeze<Mock<IItemService<Mount>>>();
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Mount, MountReadDto>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<MountBaseDto, Mount>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<MountBaseDto, Mount>>>();

            MockMountsController = new(
                MockMountService.Object,
                MockPaginatedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

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

        public MountsController MockMountsController { get; }
        public Mock<IItemService<Mount>> MockMountService { get; }
        public Mock<IMapper<PaginatedList<Mount>, PageDto<MountReadDto>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Mount, MountReadDto>> MockReadMapper { get; }
        public Mock<IMapper<MountBaseDto, Mount>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<MountBaseDto, Mount>> MockUpdateMapper { get; }

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

            MockMountsController.ControllerContext = new ControllerContext();
            MockMountsController.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = user
            };
        }

        public void MockObjectModelValidator(ControllerBase controller)
        {
            var objectValidator = new Mock<IObjectModelValidator>();

            objectValidator.Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;
        }

        public void MockModelError(ControllerBase controller)
        {
            var context = new ControllerContext(
                new ActionContext(
                    new DefaultHttpContext() { TraceIdentifier = "trace" },
                    new RouteData(),
                    new ControllerActionDescriptor()));

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
}

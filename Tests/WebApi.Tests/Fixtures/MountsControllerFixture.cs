using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.MountViewModels;
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
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Mount, MountReadViewModel>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<MountBaseViewModel, Mount>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<MountBaseViewModel, Mount>>>();

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
            MountReadViewModel = GetMountReadViewModel();
            MountBaseViewModel = GetMountBaseViewModel();
            HitViewModel = GetHitViewModel();
            PageParameters = GetPageParameters();
            PagedList = GetPagedList();
            PageViewModel = GetPageViewModel();
            PatchDocument = GetPatchDocument();
        }

        public MountsController MockMountsController { get; }
        public Mock<IItemService<Mount>> MockMountService { get; }
        public Mock<IMapper<PaginatedList<Mount>, PageViewModel<MountReadViewModel>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Mount, MountReadViewModel>> MockReadMapper { get; }
        public Mock<IMapper<MountBaseViewModel, Mount>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<MountBaseViewModel, Mount>> MockUpdateMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public Mount Mount { get; }
        public MountReadViewModel MountReadViewModel { get; }
        public MountBaseViewModel MountBaseViewModel { get; }
        public HitViewModel HitViewModel { get; }
        public PageParameters PageParameters { get; }
        public PaginatedList<Mount> PagedList { get; }
        public PageViewModel<MountReadViewModel> PageViewModel { get; }
        public JsonPatchDocument<MountBaseViewModel> PatchDocument { get; }

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

        private PaginatedList<Mount> GetPagedList()
        {
            return new PaginatedList<Mount>(GetMounts(), 6, 1, 5);
        }

        private MountBaseViewModel GetMountBaseViewModel()
        {
            return new MountBaseViewModel()
            {
                Name = Name,
                Type = MountType.Horse,
                Speed = 10
            };
        }

        private MountReadViewModel GetMountReadViewModel()
        {
            return new MountReadViewModel()
            {
                Id = Id,
                Name = Name,
                Type = MountType.Horse,
                Speed = 10
            };
        }

        private List<MountReadViewModel> GetMountReadViewModels()
        {
            return new List<MountReadViewModel>()
            {
                MountReadViewModel,
                MountReadViewModel
            };
        }

        private HitViewModel GetHitViewModel()
        {
            return new HitViewModel()
            {
                DealerId = Id,
                ItemId = Id,
                ReceiverId = Id
            };
        }

        private PageViewModel<MountReadViewModel> GetPageViewModel()
        {
            return new PageViewModel<MountReadViewModel>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetMountReadViewModels()
            };
        }

        private JsonPatchDocument<MountBaseViewModel> GetPatchDocument()
        {
            return new JsonPatchDocument<MountBaseViewModel>();
        }
    }
}

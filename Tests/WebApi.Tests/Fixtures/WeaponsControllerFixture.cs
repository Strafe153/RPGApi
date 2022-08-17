using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.WeaponViewModels;
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
    public class WeaponsControllerFixture
    {
        public WeaponsControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockWeaponService = fixture.Freeze<Mock<IItemService<Weapon>>>();
            MockCharacterService = fixture.Freeze<Mock<ICharacterService>>();
            MockPlayerService = fixture.Freeze<Mock<IPlayerService>>();
            MockPagedMapper = fixture.Freeze<Mock<IEnumerableMapper<PagedList<Weapon>, PageViewModel<WeaponReadViewModel>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Weapon, WeaponReadViewModel>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<WeaponBaseViewModel, Weapon>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<WeaponBaseViewModel, Weapon>>>();

            MockWeaponsController = new(
                MockWeaponService.Object,
                MockCharacterService.Object,
                MockPlayerService.Object,
                MockPagedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

            Id = 1;
            Name = "Name";
            Character = GetCharacter();
            Weapon = GetWeapon();
            WeaponReadViewModel = GetWeaponReadViewmModel();
            SpellBaseViewModel = GetWeaponBaseViewModel();
            HitViewModel = GetHitViewModel();
            PageParameters = GetPageParameters();
            PagedList = GetPagedList();
            PageViewModel = GetPageViewModel();
            PatchDocument = GetPatchDocument();
        }

        public WeaponsController MockWeaponsController { get; }
        public Mock<IItemService<Weapon>> MockWeaponService { get; }
        public Mock<ICharacterService> MockCharacterService { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IEnumerableMapper<PagedList<Weapon>, PageViewModel<WeaponReadViewModel>>> MockPagedMapper { get; }
        public Mock<IMapper<Weapon, WeaponReadViewModel>> MockReadMapper { get; }
        public Mock<IMapper<WeaponBaseViewModel, Weapon>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<WeaponBaseViewModel, Weapon>> MockUpdateMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public Weapon Weapon { get; }
        public WeaponReadViewModel WeaponReadViewModel { get; }
        public WeaponBaseViewModel SpellBaseViewModel { get; }
        public HitViewModel HitViewModel { get; }
        public PageParameters PageParameters { get; }
        public PagedList<Weapon> PagedList { get; }
        public PageViewModel<WeaponReadViewModel> PageViewModel { get; }
        public JsonPatchDocument<WeaponBaseViewModel> PatchDocument { get; }

        public void MockControllerBaseUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            MockWeaponsController.ControllerContext = new ControllerContext();
            MockWeaponsController.ControllerContext.HttpContext = new DefaultHttpContext()
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

        private PagedList<Weapon> GetPagedList()
        {
            return new PagedList<Weapon>(GetWeapons(), 6, 1, 5);
        }

        private WeaponBaseViewModel GetWeaponBaseViewModel()
        {
            return new WeaponBaseViewModel()
            {
                Name = Name,
                Type = WeaponType.Sword,
                Damage = 20
            };
        }

        private WeaponReadViewModel GetWeaponReadViewmModel()
        {
            return new WeaponReadViewModel()
            {
                Id = Id,
                Name = Name,
                Type = WeaponType.Sword,
                Damage = 20
            };
        }

        private List<WeaponReadViewModel> GetWeaponReadViewModels()
        {
            return new List<WeaponReadViewModel>()
            {
                WeaponReadViewModel,
                WeaponReadViewModel
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

        private PageViewModel<WeaponReadViewModel> GetPageViewModel()
        {
            return new PageViewModel<WeaponReadViewModel>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetWeaponReadViewModels()
            };
        }

        private JsonPatchDocument<WeaponBaseViewModel> GetPatchDocument()
        {
            return new JsonPatchDocument<WeaponBaseViewModel>();
        }
    }
}

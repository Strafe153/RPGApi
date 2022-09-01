using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Dtos;
using Core.Dtos.WeaponDtos;
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
    public class WeaponsControllerFixture
    {
        public WeaponsControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockWeaponService = fixture.Freeze<Mock<IItemService<Weapon>>>();
            MockCharacterService = fixture.Freeze<Mock<ICharacterService>>();
            MockPlayerService = fixture.Freeze<Mock<IPlayerService>>();
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Weapon, WeaponReadDto>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<WeaponBaseDto, Weapon>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<WeaponBaseDto, Weapon>>>();

            MockWeaponsController = new(
                MockWeaponService.Object,
                MockCharacterService.Object,
                MockPlayerService.Object,
                MockPaginatedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

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

        public WeaponsController MockWeaponsController { get; }
        public Mock<IItemService<Weapon>> MockWeaponService { get; }
        public Mock<ICharacterService> MockCharacterService { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Weapon, WeaponReadDto>> MockReadMapper { get; }
        public Mock<IMapper<WeaponBaseDto, Weapon>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<WeaponBaseDto, Weapon>> MockUpdateMapper { get; }

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
}

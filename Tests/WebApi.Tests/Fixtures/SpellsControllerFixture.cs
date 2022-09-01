using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Dtos;
using Core.Dtos.SpellDtos;
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
    public class SpellsControllerFixture
    {
        public SpellsControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockSpellService = fixture.Freeze<Mock<IItemService<Spell>>>();
            MockCharacterService = fixture.Freeze<Mock<ICharacterService>>();
            MockPlayerService = fixture.Freeze<Mock<IPlayerService>>();
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Spell, SpellReadDto>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<SpellBaseDto, Spell>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<SpellBaseDto, Spell>>>();

            MockSpellsController = new(
                MockSpellService.Object,
                MockCharacterService.Object,
                MockPlayerService.Object,
                MockPaginatedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

            Id = 1;
            Name = "Name";
            Character = GetCharacter();
            Spell = GetSpell();
            SpellReadDto = GetSpellReadDto();
            SpellBaseDto = GetSpellBaseViewModel();
            HitDto = GetHitDto();
            PageParameters = GetPageParameters();
            PaginatedList = GetPaginatedList();
            PageDto = GetPageDto();
            PatchDocument = GetPatchDocument();
        }

        public SpellsController MockSpellsController { get; }
        public Mock<IItemService<Spell>> MockSpellService { get; }
        public Mock<ICharacterService> MockCharacterService { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Spell, SpellReadDto>> MockReadMapper { get; }
        public Mock<IMapper<SpellBaseDto, Spell>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<SpellBaseDto, Spell>> MockUpdateMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public Spell Spell { get; }
        public SpellReadDto SpellReadDto { get; }
        public SpellBaseDto SpellBaseDto { get; }
        public HitDto HitDto { get; }
        public PageParameters PageParameters { get; }
        public PaginatedList<Spell> PaginatedList { get; }
        public PageDto<SpellReadDto> PageDto { get; }
        public JsonPatchDocument<SpellBaseDto> PatchDocument { get; }

        public void MockControllerBaseUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            MockSpellsController.ControllerContext = new ControllerContext();
            MockSpellsController.ControllerContext.HttpContext = new DefaultHttpContext()
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

        private Spell GetSpell()
        {
            return new Spell()
            {
                Id = Id,
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private List<Spell> GetSpells()
        {
            return new List<Spell>()
            {
                Spell,
                Spell
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

        private PaginatedList<Spell> GetPaginatedList()
        {
            return new PaginatedList<Spell>(GetSpells(), 6, 1, 5);
        }

        private SpellBaseDto GetSpellBaseViewModel()
        {
            return new SpellBaseDto()
            {
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private SpellReadDto GetSpellReadDto()
        {
            return new SpellReadDto()
            {
                Id = Id,
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private List<SpellReadDto> GetSpellReadDtos()
        {
            return new List<SpellReadDto>()
            {
                SpellReadDto,
                SpellReadDto
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

        private PageDto<SpellReadDto> GetPageDto()
        {
            return new PageDto<SpellReadDto>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetSpellReadDtos()
            };
        }

        private JsonPatchDocument<SpellBaseDto> GetPatchDocument()
        {
            return new JsonPatchDocument<SpellBaseDto>();
        }
    }
}

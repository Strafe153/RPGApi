using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
using NSubstitute;
using System.Security.Claims;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures
{
    public class SpellsControllerFixture
    {
        public SpellsControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            SpellService = fixture.Freeze<IItemService<Spell>>();
            CharacterService = fixture.Freeze<ICharacterService>();
            PlayerService = fixture.Freeze<IPlayerService>();
            PaginatedMapper = fixture.Freeze<IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>>>();
            ReadMapper = fixture.Freeze<IMapper<Spell, SpellReadDto>>();
            CreateMapper = fixture.Freeze<IMapper<SpellBaseDto, Spell>>();
            UpdateMapper = fixture.Freeze<IUpdateMapper<SpellBaseDto, Spell>>();

            SpellsController = new(
                SpellService,
                CharacterService,
                PlayerService,
                PaginatedMapper,
                ReadMapper,
                CreateMapper,
                UpdateMapper);

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

        public SpellsController SpellsController { get; }
        public IItemService<Spell> SpellService { get; }
        public ICharacterService CharacterService { get; }
        public IPlayerService PlayerService { get; }
        public IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>> PaginatedMapper { get; }
        public IMapper<Spell, SpellReadDto> ReadMapper { get; }
        public IMapper<SpellBaseDto, Spell> CreateMapper { get; }
        public IUpdateMapper<SpellBaseDto, Spell> UpdateMapper { get; }

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

            SpellsController.ControllerContext = new ControllerContext();
            SpellsController.ControllerContext.HttpContext = new DefaultHttpContext()
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

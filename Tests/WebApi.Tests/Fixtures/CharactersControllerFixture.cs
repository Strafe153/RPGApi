using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Dtos;
using Core.Dtos.CharacterDtos;
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
    public class CharactersControllerFixture
    {
        public CharactersControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            CharacterService = fixture.Freeze<ICharacterService>();
            PlayerService = fixture.Freeze<IPlayerService>();
            WeaponService = fixture.Freeze<IItemService<Weapon>>();
            SpellService = fixture.Freeze<IItemService<Spell>>();
            MountService = fixture.Freeze<IItemService<Mount>>();
            PaginatedMapper = fixture.Freeze<IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>>>();
            ReadMapper = fixture.Freeze<IMapper<Character, CharacterReadDto>>();
            CreateMapper = fixture.Freeze<IMapper<CharacterCreateDto, Character>>();
            UpdateMapper = fixture.Freeze<IUpdateMapper<CharacterBaseDto, Character>>();

            CharactersController = new(
                CharacterService,
                PlayerService,
                WeaponService,
                SpellService,
                MountService,
                PaginatedMapper,
                ReadMapper,
                CreateMapper,
                UpdateMapper);

            Id = 1;
            Name = "Name";
            Weapon = GetWeapon();
            Spell = GetSpell();
            Mount = GetMount();
            Character = GetCharacter();
            CharacterReadDto = GetCharacterReadDto();
            CharacterCreateDto = GetCharacterCreateDto();
            CharacterUpdateDto = GetCharacterBaseDto();
            PatchDocument = GetPatchDocument();
            ItemDto = GetItemDto();
            PageParameters = GetPageParameters();
            PaginatedList = GetPaginatedList();
            PageDto = GetPageDto();
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
        public string? Name { get; }
        public Weapon Weapon { get; }
        public Spell Spell { get; }
        public Mount Mount { get; }
        public Character Character { get; }
        public CharacterReadDto CharacterReadDto { get; }
        public CharacterCreateDto CharacterCreateDto { get; }
        public CharacterBaseDto CharacterUpdateDto { get; }
        public JsonPatchDocument<CharacterBaseDto> PatchDocument { get; }
        public AddRemoveItemDto ItemDto { get; }
        public PageParameters PageParameters { get; }
        public PaginatedList<Character> PaginatedList { get; }
        public PageDto<CharacterReadDto> PageDto { get; }

        public void MockControllerBaseUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            CharactersController.ControllerContext = new ControllerContext();
            CharactersController.ControllerContext.HttpContext = new DefaultHttpContext() 
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
                    new DefaultHttpContext()
                    {
                        TraceIdentifier = "trace"
                    },
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

        private Weapon GetWeapon()
        {
            return new Weapon()
            {
                Id = Id,
                Name = Name,
                Damage = Id,
                Type = WeaponType.Sword
            };
        }

        private Spell GetSpell()
        {
            return new Spell()
            {
                Id = Id,
                Name = Name,
                Damage = Id,
                Type = SpellType.Fire
            };
        }

        private Mount GetMount()
        {
            return new Mount()
            {
                Id = Id,
                Name = Name,
                Speed = Id,
                Type = MountType.Horse
            };
        }

        private Character GetCharacter()
        {
            return new Character()
            {
                Id = Id,
                Name = Name,
                Health = 100,
                Race = CharacterRace.Human,
                PlayerId = Id
            };
        }

        private List<Character> GetCharacters()
        {
            return new List<Character>()
            {
                Character,
                Character
            };
        }

        private CharacterReadDto GetCharacterReadDto()
        {
            return new CharacterReadDto()
            {
                Id = Id,
                Name = Name,
                Health = 100,
                Race = CharacterRace.Human
            };
        }

        private CharacterCreateDto GetCharacterCreateDto()
        {
            return new CharacterCreateDto()
            {
                Name = Name,
                Race = CharacterRace.Human,
                PlayerId = Id
            };
        }

        private CharacterBaseDto GetCharacterBaseDto()
        {
            return new CharacterBaseDto()
            {
                Name = Name,
                Race = CharacterRace.Human
            };
        }

        private JsonPatchDocument<CharacterBaseDto> GetPatchDocument()
        {
            return new JsonPatchDocument<CharacterBaseDto>();
        }

        private AddRemoveItemDto GetItemDto()
        {
            return new AddRemoveItemDto()
            {
                CharacterId = Id,
                ItemId = Id
            };
        }

        private List<CharacterReadDto> GetCharacterReadDtos()
        {
            return new List<CharacterReadDto>()
            {
                CharacterReadDto,
                CharacterReadDto
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

        private PaginatedList<Character> GetPaginatedList()
        {
            return new PaginatedList<Character>(GetCharacters(), 6, 1, 5);
        }

        private PageDto<CharacterReadDto> GetPageDto()
        {
            return new PageDto<CharacterReadDto>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetCharacterReadDtos()
            };
        }
    }
}

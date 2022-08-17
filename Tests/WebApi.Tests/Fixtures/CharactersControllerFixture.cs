using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
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
    public class CharactersControllerFixture
    {
        public CharactersControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockCharacterService = fixture.Freeze<Mock<ICharacterService>>();
            MockPlayerService = fixture.Freeze<Mock<IPlayerService>>();
            MockWeaponService = fixture.Freeze<Mock<IItemService<Weapon>>>();
            MockSpellService = fixture.Freeze<Mock<IItemService<Spell>>>();
            MockMountService = fixture.Freeze<Mock<IItemService<Mount>>>();
            MockPagedMapper = fixture.Freeze<Mock<IEnumerableMapper<PagedList<Character>, PageViewModel<CharacterReadViewModel>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Character, CharacterReadViewModel>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<CharacterCreateViewModel, Character>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<CharacterBaseViewModel, Character>>>();

            MockCharactersController = new(
                MockCharacterService.Object,
                MockPlayerService.Object,
                MockWeaponService.Object,
                MockSpellService.Object,
                MockMountService.Object,
                MockPagedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

            Id = 1;
            Name = "Name";
            Weapon = GetWeapon();
            Spell = GetSpell();
            Mount = GetMount();
            Character = GetCharacter();
            CharacterReadViewModel = GetCharacterReadViewModel();
            CharacterCreateViewModel = GetCharacterCreateViewModel();
            CharacterUpdateViewModel = GetCharacterBaseViewModel();
            PatchDocument = GetPatchDocument();
            ItemViewModel = GetItemViewModel();
            PageParameters = GetPageParameters();
            PagedList = GetPagedList();
            PageViewModel = GetPageViewModel();
        }

        public CharactersController MockCharactersController { get; }
        public Mock<ICharacterService> MockCharacterService { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IItemService<Weapon>> MockWeaponService { get; }
        public Mock<IItemService<Spell>> MockSpellService { get; }
        public Mock<IItemService<Mount>> MockMountService { get; }
        public Mock<IEnumerableMapper<PagedList<Character>, PageViewModel<CharacterReadViewModel>>> MockPagedMapper { get; }
        public Mock<IMapper<Character, CharacterReadViewModel>> MockReadMapper { get; }
        public Mock<IMapper<CharacterCreateViewModel, Character>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<CharacterBaseViewModel, Character>> MockUpdateMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public Weapon Weapon { get; }
        public Spell Spell { get; }
        public Mount Mount { get; }
        public Character Character { get; }
        public CharacterReadViewModel CharacterReadViewModel { get; }
        public CharacterCreateViewModel CharacterCreateViewModel { get; }
        public CharacterBaseViewModel CharacterUpdateViewModel { get; }
        public JsonPatchDocument<CharacterBaseViewModel> PatchDocument { get; }
        public AddRemoveItemViewModel ItemViewModel { get; }
        public PageParameters PageParameters { get; }
        public PagedList<Character> PagedList { get; }
        public PageViewModel<CharacterReadViewModel> PageViewModel { get; }

        public void MockControllerBaseUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            MockCharactersController.ControllerContext = new ControllerContext();
            MockCharactersController.ControllerContext.HttpContext = new DefaultHttpContext() 
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
                    new DefaultHttpContext() 
                    { 
                        TraceIdentifier = "trace" 
                    },
                    new RouteData(),
                    new ControllerActionDescriptor()));

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

        private CharacterReadViewModel GetCharacterReadViewModel()
        {
            return new CharacterReadViewModel()
            {
                Id = Id,
                Name = Name,
                Health = 100,
                Race = CharacterRace.Human
            };
        }

        private CharacterCreateViewModel GetCharacterCreateViewModel()
        {
            return new CharacterCreateViewModel()
            {
                Name = Name,
                Race = CharacterRace.Human,
                PlayerId = Id
            };
        }

        private CharacterBaseViewModel GetCharacterBaseViewModel()
        {
            return new CharacterBaseViewModel()
            {
                Name = Name,
                Race = CharacterRace.Human
            };
        }

        private JsonPatchDocument<CharacterBaseViewModel> GetPatchDocument()
        {
            return new JsonPatchDocument<CharacterBaseViewModel>();
        }

        private AddRemoveItemViewModel GetItemViewModel()
        {
            return new AddRemoveItemViewModel()
            {
                CharacterId = Id,
                ItemId = Id
            };
        }

        private List<CharacterReadViewModel> GetCharacterReadViewModels()
        {
            return new List<CharacterReadViewModel>()
            {
                CharacterReadViewModel,
                CharacterReadViewModel
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

        private PagedList<Character> GetPagedList()
        {
            return new PagedList<Character>(GetCharacters(), 6, 1, 5);
        }

        private PageViewModel<CharacterReadViewModel> GetPageViewModel()
        {
            return new PageViewModel<CharacterReadViewModel>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetCharacterReadViewModels()
            };
        }
    }
}

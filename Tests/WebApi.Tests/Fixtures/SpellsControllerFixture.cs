using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.SpellViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing;
using Moq;
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
            MockPagedMapper = fixture.Freeze<Mock<IEnumerableMapper<PagedList<Spell>, PageViewModel<SpellReadViewModel>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Spell, SpellReadViewModel>>>();
            MockCreateMapper = fixture.Freeze<Mock<IMapper<SpellBaseViewModel, Spell>>>();
            MockUpdateMapper = fixture.Freeze<Mock<IUpdateMapper<SpellBaseViewModel, Spell>>>();

            MockSpellsController = new(
                MockSpellService.Object,
                MockCharacterService.Object,
                MockPlayerService.Object,
                MockPagedMapper.Object,
                MockReadMapper.Object,
                MockCreateMapper.Object,
                MockUpdateMapper.Object);

            Id = 1;
            Name = "Name";
            Character = GetCharacter();
            Spell = GetSpell();
            SpellReadViewModel = GetSpellReadViewModel();
            SpellBaseViewModel = GetSpellBaseViewModel();
            HitViewModel = GetHitViewModel();
            PageParameters = GetPageParameters();
            PagedList = GetPagedList();
            PageViewModel = GetPageViewModel();
            PatchDocument = GetPatchDocument();
        }

        public SpellsController MockSpellsController { get; }
        public Mock<IItemService<Spell>> MockSpellService { get; }
        public Mock<ICharacterService> MockCharacterService { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IEnumerableMapper<PagedList<Spell>, PageViewModel<SpellReadViewModel>>> MockPagedMapper { get; }
        public Mock<IMapper<Spell, SpellReadViewModel>> MockReadMapper { get; }
        public Mock<IMapper<SpellBaseViewModel, Spell>> MockCreateMapper { get; }
        public Mock<IUpdateMapper<SpellBaseViewModel, Spell>> MockUpdateMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public Character Character { get; }
        public Spell Spell { get; }
        public SpellReadViewModel SpellReadViewModel { get; }
        public SpellBaseViewModel SpellBaseViewModel { get; }
        public HitViewModel HitViewModel { get; }
        public PageParameters PageParameters { get; }
        public PagedList<Spell> PagedList { get; }
        public PageViewModel<SpellReadViewModel> PageViewModel { get; }
        public JsonPatchDocument<SpellBaseViewModel> PatchDocument { get; }

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

        private PagedList<Spell> GetPagedList()
        {
            return new PagedList<Spell>(GetSpells(), 6, 1, 5);
        }

        private SpellBaseViewModel GetSpellBaseViewModel()
        {
            return new SpellBaseViewModel()
            {
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private SpellReadViewModel GetSpellReadViewModel()
        {
            return new SpellReadViewModel()
            {
                Id = Id,
                Name = Name,
                Type = SpellType.Fire,
                Damage = 20
            };
        }

        private List<SpellReadViewModel> GetSpellReadViewModels()
        {
            return new List<SpellReadViewModel>()
            {
                SpellReadViewModel,
                SpellReadViewModel
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

        private PageViewModel<SpellReadViewModel> GetPageViewModel()
        {
            return new PageViewModel<SpellReadViewModel>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetSpellReadViewModels()
            };
        }

        private JsonPatchDocument<SpellBaseViewModel> GetPatchDocument()
        {
            return new JsonPatchDocument<SpellBaseViewModel>();
        }
    }
}

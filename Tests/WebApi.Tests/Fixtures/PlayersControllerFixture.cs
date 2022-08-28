using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.PlayerViewModels;
using Moq;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures
{
    public class PlayersControllerFixture
    {
        public PlayersControllerFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            MockPlayerService = fixture.Freeze<Mock<IPlayerService>>();
            MockPasswordService = fixture.Freeze<Mock<IPasswordService>>();
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Player>, PageViewModel<PlayerReadViewModel>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Player, PlayerReadViewModel>>>();
            MockReadWithTokenMapper = fixture.Freeze<Mock<IMapper<Player, PlayerWithTokenReadViewModel>>>();

            MockPlayersController = new(
                MockPlayerService.Object,
                MockPasswordService.Object,
                MockPaginatedMapper.Object,
                MockReadMapper.Object,
                MockReadWithTokenMapper.Object);

            Id = 1;
            Name = "Name";
            Bytes = new byte[0];
            Player = GetPlayer();
            PlayerReadViewModel = GetPlayerReadViewModel();
            PlayerWithTokenReadViewModel = GetPlayerWithTokenReadViewModel();
            PlayerAuthorizeViewModel = GetPlayerAuthorizeViewModel();
            PlayerUpdateViewModel = GetPlayerUpdateViewModel();
            PlayerChangePasswordViewModel = GetPlayerChangePasswordViewModel();
            PlayerChangeRoleViewModel = GetPlayerChangeRoleViewModel();
            PageParameters = GetPageParameters();
            PagedList = GetPagedList();
            PageViewModel = GetPageViewModel();
        }

        public PlayersController MockPlayersController { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IPasswordService> MockPasswordService { get; }
        public Mock<IMapper<PaginatedList<Player>, PageViewModel<PlayerReadViewModel>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Player, PlayerReadViewModel>> MockReadMapper { get; }
        public Mock<IMapper<Player, PlayerWithTokenReadViewModel>> MockReadWithTokenMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public byte[] Bytes { get; }
        public Player Player { get; }
        public PlayerReadViewModel PlayerReadViewModel { get; }
        public PlayerWithTokenReadViewModel PlayerWithTokenReadViewModel { get; }
        public PlayerAuthorizeViewModel PlayerAuthorizeViewModel { get; }
        public PlayerBaseViewModel PlayerUpdateViewModel { get; }
        public PlayerChangePasswordViewModel PlayerChangePasswordViewModel { get; }
        public PlayerChangeRoleViewModel PlayerChangeRoleViewModel { get; }
        public PageParameters PageParameters { get; }
        public PaginatedList<Player> PagedList { get; }
        public PageViewModel<PlayerReadViewModel> PageViewModel { get; }

        private Player GetPlayer()
        {
            return new Player()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player,
                PasswordHash = Bytes,
                PasswordSalt = Bytes
            };
        }

        private List<Player> GetPlayers()
        {
            return new List<Player>()
            {
                Player,
                Player
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

        private PlayerReadViewModel GetPlayerReadViewModel()
        {
            return new PlayerReadViewModel()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player
            };
        }

        private List<PlayerReadViewModel> GetPlayerReadViewModels()
        {
            return new List<PlayerReadViewModel>()
            {
                PlayerReadViewModel,
                PlayerReadViewModel
            };
        }

        private PlayerWithTokenReadViewModel GetPlayerWithTokenReadViewModel()
        {
            return new PlayerWithTokenReadViewModel()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player,
                Token = Name
            };
        }

        private PlayerAuthorizeViewModel GetPlayerAuthorizeViewModel()
        {
            return new PlayerAuthorizeViewModel()
            {
                Name = Name,
                Password = Name
            };
        }

        private PlayerBaseViewModel GetPlayerUpdateViewModel()
        {
            return new PlayerBaseViewModel()
            {
                Name = Name
            };
        }

        private PlayerChangePasswordViewModel GetPlayerChangePasswordViewModel()
        {
            return new PlayerChangePasswordViewModel()
            {
                Password = Name
            };
        }

        private PlayerChangeRoleViewModel GetPlayerChangeRoleViewModel()
        {
            return new PlayerChangeRoleViewModel()
            {
                Role = PlayerRole.Admin
            };
        }

        private PaginatedList<Player> GetPagedList()
        {
            return new PaginatedList<Player>(GetPlayers(), 6, 1, 5);
        }

        private PageViewModel<PlayerReadViewModel> GetPageViewModel()
        {
            return new PageViewModel<PlayerReadViewModel>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetPlayerReadViewModels()
            };
        }
    }
}

using AutoFixture;
using AutoFixture.AutoMoq;
using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
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
            MockPaginatedMapper = fixture.Freeze<Mock<IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>>>>();
            MockReadMapper = fixture.Freeze<Mock<IMapper<Player, PlayerReadDto>>>();
            MockReadWithTokenMapper = fixture.Freeze<Mock<IMapper<Player, PlayerWithTokenReadDto>>>();

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
            PlayerReadDto = GetPlayerReadDto();
            PlayerWithTokenReadDto = GetPlayerWithTokenReadDto();
            PlayerAuthorizeDto = GetPlayerAuthorizeDto();
            PlayerUpdateDto = GetPlayerUpdateDto();
            PlayerChangePasswordDto = GetPlayerChangePasswordDto();
            PlayerChangeRoleDto = GetPlayerChangeRoleDto();
            PageParameters = GetPageParameters();
            PaginatedList = GetPaginatedList();
            PageDto = GetPageDto();
        }

        public PlayersController MockPlayersController { get; }
        public Mock<IPlayerService> MockPlayerService { get; }
        public Mock<IPasswordService> MockPasswordService { get; }
        public Mock<IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>>> MockPaginatedMapper { get; }
        public Mock<IMapper<Player, PlayerReadDto>> MockReadMapper { get; }
        public Mock<IMapper<Player, PlayerWithTokenReadDto>> MockReadWithTokenMapper { get; }

        public int Id { get; }
        public string? Name { get; }
        public byte[] Bytes { get; }
        public Player Player { get; }
        public PlayerReadDto PlayerReadDto { get; }
        public PlayerWithTokenReadDto PlayerWithTokenReadDto { get; }
        public PlayerAuthorizeDto PlayerAuthorizeDto { get; }
        public PlayerBaseDto PlayerUpdateDto { get; }
        public PlayerChangePasswordDto PlayerChangePasswordDto { get; }
        public PlayerChangeRoleDto PlayerChangeRoleDto { get; }
        public PageParameters PageParameters { get; }
        public PaginatedList<Player> PaginatedList { get; }
        public PageDto<PlayerReadDto> PageDto { get; }

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

        private PlayerReadDto GetPlayerReadDto()
        {
            return new PlayerReadDto()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player
            };
        }

        private List<PlayerReadDto> GetPlayerReadDtos()
        {
            return new List<PlayerReadDto>()
            {
                PlayerReadDto,
                PlayerReadDto
            };
        }

        private PlayerWithTokenReadDto GetPlayerWithTokenReadDto()
        {
            return new PlayerWithTokenReadDto()
            {
                Id = Id,
                Name = Name,
                Role = PlayerRole.Player,
                Token = Name
            };
        }

        private PlayerAuthorizeDto GetPlayerAuthorizeDto()
        {
            return new PlayerAuthorizeDto()
            {
                Name = Name,
                Password = Name
            };
        }

        private PlayerBaseDto GetPlayerUpdateDto()
        {
            return new PlayerBaseDto()
            {
                Name = Name
            };
        }

        private PlayerChangePasswordDto GetPlayerChangePasswordDto()
        {
            return new PlayerChangePasswordDto()
            {
                Password = Name
            };
        }

        private PlayerChangeRoleDto GetPlayerChangeRoleDto()
        {
            return new PlayerChangeRoleDto()
            {
                Role = PlayerRole.Admin
            };
        }

        private PaginatedList<Player> GetPaginatedList()
        {
            return new PaginatedList<Player>(GetPlayers(), 6, 1, 5);
        }

        private PageDto<PlayerReadDto> GetPageDto()
        {
            return new PageDto<PlayerReadDto>()
            {
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 5,
                TotalItems = 6,
                HasPrevious = false,
                HasNext = true,
                Entities = GetPlayerReadDtos()
            };
        }
    }
}

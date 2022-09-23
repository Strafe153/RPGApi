using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Core.Models;
using WebApi.Controllers;
using WebApi.Mappers.Interfaces;

namespace WebApi.Tests.Fixtures;

public class PlayersControllerFixture
{
    public PlayersControllerFixture()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        PlayerService = fixture.Freeze<IPlayerService>();
        PasswordService = fixture.Freeze<IPasswordService>();
        PaginatedMapper = fixture.Freeze<IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>>>();
        ReadMapper = fixture.Freeze<IMapper<Player, PlayerReadDto>>();
        ReadWithTokenMapper = fixture.Freeze<IMapper<Player, PlayerWithTokenReadDto>>();

        PlayerContainer = new(
            PlayerService,
            PasswordService,
            PaginatedMapper,
            ReadMapper,
            ReadWithTokenMapper);

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

    public PlayersController PlayerContainer { get; }
    public IPlayerService PlayerService { get; }
    public IPasswordService PasswordService { get; }
    public IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> PaginatedMapper { get; }
    public IMapper<Player, PlayerReadDto> ReadMapper { get; }
    public IMapper<Player, PlayerWithTokenReadDto> ReadWithTokenMapper { get; }

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

using Core.Dtos;
using Core.Dtos.PlayerDtos;
using Core.Dtos.TokensDtos;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers;

[Route("api/players")]
[ApiController]
[Authorize]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> _paginatedMapper;
    private readonly IMapper<Player, PlayerReadDto> _readMapper;

    public PlayersController(
        IPlayerService playerService,
        IPasswordService passwordService,
        ITokenService tokenService,
        IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> paginatedMapper,
        IMapper<Player, PlayerReadDto> readMapper)
    {
        _playerService = playerService;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
    }

    [HttpGet]
    public async Task<ActionResult<PageDto<PlayerReadDto>>> GetAsync([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var players = await _playerService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(players);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<PlayerReadDto>> GetAsync([FromRoute] int id, CancellationToken token)
    {
        var player = await _playerService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(player);

        return Ok(readDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerReadDto>> RegisterAsync([FromBody] PlayerAuthorizeDto authorizeModel)
    {
        (byte[] hash, byte[] salt) = _passwordService.GeneratePasswordHashAndSalt(authorizeModel.Password!);

        Player player = _playerService.CreatePlayer(authorizeModel.Name!, hash, salt);
        player.Id = await _playerService.AddAsync(player);

        var readDto = _readMapper.Map(player);

        return CreatedAtAction(nameof(GetAsync), new { Id = readDto.Id }, readDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensReadDto>> LoginAsync([FromBody] PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var player = await _playerService.GetByNameAsync(authorizeDto.Name!, token);
        _passwordService.VerifyPasswordHash(authorizeDto.Password!, player);

        string accessToken = _tokenService.GenerateAccessToken(player);
        string refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(refreshToken, player, Response);
        await _playerService.UpdateAsync(player);

        var tokensDto = new TokensReadDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Ok(tokensDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] PlayerBaseDto updateDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);

        player.Name = updateDto.Name;
        await _playerService.UpdateAsync(player);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var player = await _playerService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(player);
        await _playerService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{id:int:min(1)}/changePassword")]
    public async Task<ActionResult<TokensReadDto>> ChangePasswordAsync(
        [FromRoute] int id, 
        [FromBody] PlayerChangePasswordDto changePasswordDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);

        (byte[] hash, byte[] salt) = _passwordService.GeneratePasswordHashAndSalt(changePasswordDto.Password!);
        _playerService.ChangePasswordData(player, hash, salt);

        string accessToken = _tokenService.GenerateAccessToken(player);
        string refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(refreshToken, player, Response);
        await _playerService.UpdateAsync(player);

        var tokensDto = new TokensReadDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Ok(tokensDto);
    }

    [HttpPut("{id:int:min(1)}/changeRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PlayerReadDto>> ChangeRoleAsync([FromRoute] int id, [FromBody] PlayerChangeRoleDto changeRoleDto)
    {
        var player = await _playerService.GetByIdAsync(id);

        player.Role = changeRoleDto.Role;
        await _playerService.UpdateAsync(player);

        var readDto = _readMapper.Map(player);

        return Ok(readDto);
    }

    [HttpPut("refreshTokens/{id}")]
    public async Task<ActionResult<TokensReadDto>> RefreshTokensAsync([FromRoute] int id, [FromBody] TokensRefreshDto refreshDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);
        _tokenService.ValidateRefreshToken(player, refreshDto.RefreshToken!);

        string accessToken = _tokenService.GenerateAccessToken(player);
        string refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(refreshToken, player, Response);
        await _playerService.UpdateAsync(player);

        var tokensReadDto = new TokensReadDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Ok(tokensReadDto);
    }
}

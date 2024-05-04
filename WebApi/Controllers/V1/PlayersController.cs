using Asp.Versioning;
using Domain.Constants;
using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.TokensDtos;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Shared;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers.V1;

[Route("api/players")]
[ApiController]
[Authorize]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
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
    [CacheFilter]
    public async Task<ActionResult<PageDto<PlayerReadDto>>> Get([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var players = await _playerService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(players);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<PlayerReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var player = await _playerService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(player);

        return Ok(readDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerReadDto>> Register([FromBody] PlayerAuthorizeDto authorizeModel)
    {
        var (hash, salt) = _passwordService.GeneratePasswordHashAndSalt(authorizeModel.Password!);

        var player = _playerService.CreatePlayer(authorizeModel.Name!, hash, salt);
        player.Id = await _playerService.AddAsync(player);

        var readDto = _readMapper.Map(player);

        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensReadDto>> Login([FromBody] PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var player = await _playerService.GetByNameAsync(authorizeDto.Name!, token);
        _passwordService.VerifyPasswordHash(authorizeDto.Password!, player);

        var accessToken = _tokenService.GenerateAccessToken(player);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(player, refreshToken);
        await _playerService.UpdateAsync(player);

        var tokensDto = new TokensReadDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Ok(tokensDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] PlayerBaseDto updateDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);

        player.Name = updateDto.Name;
        await _playerService.UpdateAsync(player);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var player = await _playerService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(player);
        await _playerService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("{id:int:min(1)}/changePassword")]
    public async Task<ActionResult<TokensReadDto>> ChangePassword(
        [FromRoute] int id,
        [FromBody] PlayerChangePasswordDto changePasswordDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);

        var (hash, salt) = _passwordService.GeneratePasswordHashAndSalt(changePasswordDto.Password!);
        _playerService.ChangePasswordData(player, hash, salt);

        string accessToken = _tokenService.GenerateAccessToken(player);
        string refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(player, refreshToken);
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
    public async Task<ActionResult<PlayerReadDto>> ChangeRole([FromRoute] int id, [FromBody] PlayerChangeRoleDto changeRoleDto)
    {
        var player = await _playerService.GetByIdAsync(id);

        player.Role = changeRoleDto.Role;
        await _playerService.UpdateAsync(player);

        var readDto = _readMapper.Map(player);

        return Ok(readDto);
    }

    [HttpPut("refreshTokens/{id}")]
    public async Task<ActionResult<TokensReadDto>> RefreshTokens([FromRoute] int id, [FromBody] TokensRefreshDto refreshDto)
    {
        var player = await _playerService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(player);
        _tokenService.ValidateRefreshToken(player, refreshDto.RefreshToken!);

        var accessToken = _tokenService.GenerateAccessToken(player);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _tokenService.SetRefreshToken(player, refreshToken);
        await _playerService.UpdateAsync(player);

        var tokensReadDto = new TokensReadDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Ok(tokensReadDto);
    }
}

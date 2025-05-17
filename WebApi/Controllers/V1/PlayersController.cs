using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Services.Abstractions;
using Asp.Versioning;
using Domain.Constants;
using Domain.Enums;
using Domain.Shared;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;

namespace WebApi.Controllers.V1;

[Route("api/players")]
[ApiController]
[Authorize]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class PlayersController : ControllerBase
{
    private readonly IPlayersService _playersService;

    public PlayersController(IPlayersService playersService)
    {
        _playersService = playersService;
    }

    [HttpGet]
    [CacheFilter]
    public async Task<ActionResult<PageDto<PlayerReadDto>>> Get(
        [FromQuery] PageParameters pageParameters,
        CancellationToken token)
    {
        var players = await _playersService.GetAllAsync(pageParameters, token);
        return Ok(players);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<PlayerReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var player = await _playersService.GetByIdAsync(id, token);
        return Ok(player);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerReadDto>> Register([FromBody] PlayerAuthorizeDto authorizeDto)
    {
        var readDto = await _playersService.AddAsync(authorizeDto);
        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensReadDto>> Login([FromBody] PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var tokensDto = await _playersService.LoginAsync(authorizeDto, token);
        return Ok(tokensDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] PlayerUpdateDto updateDto, CancellationToken token)
    {
        await _playersService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await _playersService.DeleteAsync(id, token);
        return NoContent();
    }

    [HttpPut("{id:int:min(1)}/changePassword")]
    public async Task<ActionResult<TokensReadDto>> ChangePassword(
        [FromRoute] int id,
        [FromBody] PlayerChangePasswordDto changePasswordDto,
        CancellationToken token)
    {
        var tokens = await _playersService.ChangePasswordAsync(id, changePasswordDto, token);
        return Ok(tokens);
    }

    [HttpPut("{id:int:min(1)}/changeRole")]
    [Authorize(Roles = nameof(PlayerRole.Admin))]
    public async Task<ActionResult<PlayerReadDto>> ChangeRole(
        [FromRoute] int id,
        [FromBody] PlayerChangeRoleDto changeRoleDto,
        CancellationToken token)
    {
        var player = await _playersService.ChangeRoleAsync(id, changeRoleDto, token);
        return Ok(player);
    }

    [HttpPut("{id:int:min(1)}/refreshTokens")]
    public async Task<ActionResult<TokensReadDto>> RefreshTokens(
        [FromRoute] int id,
        [FromBody] TokensRefreshDto refreshDto,
        CancellationToken token)
    {
        var tokens = await _playersService.RefreshTokensAsync(id, refreshDto, token);
        return Ok(tokens);
    }
}

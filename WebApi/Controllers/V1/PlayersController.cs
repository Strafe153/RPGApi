using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Services.Abstractions;
using Application.Shared;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Constants;
using WebApi.Filters;

namespace WebApi.Controllers.V1;

[Route("api/players")]
[ApiController]
[Authorize]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class PlayersController(IPlayersService playersService) : ControllerBase
{
    [HttpGet]
    [CacheFilter]
    public async Task<ActionResult<PageDto<PlayerReadDto>>> Get([FromQuery] PageParameters pageParameters)
    {
        var players = await playersService.GetAllAsync(pageParameters);
        return Ok(players);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<PlayerReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var player = await playersService.GetByIdAsync(id, token);
        return Ok(player);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerReadDto>> Register([FromBody] PlayerAuthorizeDto authorizeDto)
    {
        var readDto = await playersService.AddAsync(authorizeDto);
        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokensReadDto>> Login([FromBody] PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var tokensDto = await playersService.LoginAsync(authorizeDto, token);
        return Ok(tokensDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] PlayerUpdateDto updateDto, CancellationToken token)
    {
        await playersService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await playersService.DeleteAsync(id, token);
        return NoContent();
    }

    [HttpPut("{id:int:min(1)}/changePassword")]
    public async Task<ActionResult<TokensReadDto>> ChangePassword(
        [FromRoute] int id,
        [FromBody] PlayerChangePasswordDto changePasswordDto,
        CancellationToken token)
    {
        var tokens = await playersService.ChangePasswordAsync(id, changePasswordDto, token);
        return Ok(tokens);
    }

    [HttpPut("{id:int:min(1)}/changeRole")]
    [Authorize(Roles = nameof(PlayerRole.Admin))]
    public async Task<ActionResult<PlayerReadDto>> ChangeRole(
        [FromRoute] int id,
        [FromBody] PlayerChangeRoleDto changeRoleDto,
        CancellationToken token)
    {
        var player = await playersService.ChangeRoleAsync(id, changeRoleDto, token);
        return Ok(player);
    }

    [HttpPut("{id:int:min(1)}/refreshTokens")]
    public async Task<ActionResult<TokensReadDto>> RefreshTokens(
        [FromRoute] int id,
        [FromBody] TokensRefreshDto refreshDto,
        CancellationToken token)
    {
        var tokens = await playersService.RefreshTokensAsync(id, refreshDto, token);
        return Ok(tokens);
    }
}

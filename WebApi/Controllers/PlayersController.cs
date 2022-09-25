using Core.Dtos;
using Core.Dtos.PlayerDtos;
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
    private readonly IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> _paginatedMapper;
    private readonly IMapper<Player, PlayerReadDto> _readMapper;
    private readonly IMapper<Player, PlayerWithTokenReadDto> _readWithTokenMapper;

    public PlayersController(
        IPlayerService playerService,
        IPasswordService passwordService,
        IMapper<PaginatedList<Player>, PageDto<PlayerReadDto>> paginatedMapper,
        IMapper<Player, PlayerReadDto> readMapper,
        IMapper<Player, PlayerWithTokenReadDto> readWithTokenMapper)
    {
        _playerService = playerService;
        _passwordService = passwordService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
        _readWithTokenMapper = readWithTokenMapper;
    }

    [HttpGet]
    public async Task<ActionResult<PageDto<PlayerReadDto>>> GetAsync([FromQuery] PageParameters pageParams)
    {
        var players = await _playerService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
        var pageDto = _paginatedMapper.Map(players);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<PlayerReadDto>> GetAsync([FromRoute] int id)
    {
        var player = await _playerService.GetByIdAsync(id);
        var readDto = _readMapper.Map(player);

        return Ok(readDto);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerReadDto>> RegisterAsync(
        [FromBody] PlayerAuthorizeDto authorizeModel)
    {
        (byte[] hash, byte[] salt) = _passwordService.CreatePasswordHash(authorizeModel.Password!);

        Player player = _playerService.CreatePlayer(authorizeModel.Name!, hash, salt);
        await _playerService.AddAsync(player);

        var readDto = _readMapper.Map(player);

        return CreatedAtAction(nameof(GetAsync), new { Id = readDto.Id }, readDto);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<PlayerWithTokenReadDto>> LoginAsync([FromBody] PlayerAuthorizeDto authorizeDto)
    {
        var player = await _playerService.GetByNameAsync(authorizeDto.Name!);
        _passwordService.VerifyPasswordHash(authorizeDto.Password!, player.PasswordHash!, player.PasswordSalt!);

        string token = _passwordService.CreateToken(player);
        var readDto = _readWithTokenMapper.Map(player) with { Token = token };

        return Ok(readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] PlayerBaseDto updateDto)
    {
        var player = await _playerService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);
        player.Name = updateDto.Name;
        await _playerService.UpdateAsync(player);

        return NoContent();
    }

    [HttpPut("{id:int:min(1)}/changePassword")]
    public async Task<ActionResult<string>> ChangePasswordAsync(
        [FromRoute] int id, 
        [FromBody] PlayerChangePasswordDto changePasswordDto)
    {
        var player = await _playerService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);

        (byte[] hash, byte[] salt) = _passwordService.CreatePasswordHash(changePasswordDto.Password!);
        _playerService.ChangePasswordData(player, hash, salt);
        await _playerService.UpdateAsync(player);

        string token = _passwordService.CreateToken(player);

        return Ok(token);
    }

    [HttpPut("{id:int:min(1)}/changeRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ChangeRoleAsync([FromRoute] int id, [FromBody] PlayerChangeRoleDto changeRoleDto)
    {
        var player = await _playerService.GetByIdAsync(id);

        player.Role = changeRoleDto.Role;
        await _playerService.UpdateAsync(player);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var player = await _playerService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);
        await _playerService.DeleteAsync(player);

        return NoContent();
    }
}

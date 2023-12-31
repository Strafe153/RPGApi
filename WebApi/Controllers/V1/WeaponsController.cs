using Core.Constants;
using Core.Dtos;
using Core.Dtos.WeaponDtos;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers.V1;

[Route("api/weapons")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class WeaponsController : ControllerBase
{
    private readonly IItemService<Weapon> _weaponService;
    private readonly ICharacterService _characterService;
    private readonly IPlayerService _playerService;
    private readonly IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>> _paginatedMapper;
    private readonly IMapper<Weapon, WeaponReadDto> _readMapper;
    private readonly IMapper<WeaponBaseDto, Weapon> _createMapper;
    private readonly IUpdateMapper<WeaponBaseDto, Weapon> _updateMapper;

    public WeaponsController(
        IItemService<Weapon> weaponService,
        ICharacterService characterService,
        IPlayerService playerService,
        IMapper<PaginatedList<Weapon>, PageDto<WeaponReadDto>> paginatedMapper,
        IMapper<Weapon, WeaponReadDto> readMapper,
        IMapper<WeaponBaseDto, Weapon> createMapper,
        IUpdateMapper<WeaponBaseDto, Weapon> updateMapper)
    {
        _weaponService = weaponService;
        _characterService = characterService;
        _playerService = playerService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
        _createMapper = createMapper;
        _updateMapper = updateMapper;
    }

    [HttpGet]
    [CacheFilter]
    public async Task<ActionResult<PageDto<WeaponReadDto>>> Get([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var weapons = await _weaponService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(weapons);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<WeaponReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var weapon = await _weaponService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(weapon);

        return Ok(readDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<WeaponReadDto>> Create([FromBody] WeaponBaseDto createDto)
    {
        var weapon = _createMapper.Map(createDto);
        weapon.Id = await _weaponService.AddAsync(weapon);

        var readDto = _readMapper.Map(weapon);

        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] WeaponBaseDto updateDto)
    {
        var weapon = await _weaponService.GetByIdAsync(id);

        _updateMapper.Map(updateDto, weapon);
        await _weaponService.UpdateAsync(weapon);

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<WeaponBaseDto> patchDocument)
    {
        var weapon = await _weaponService.GetByIdAsync(id);
        var updateDto = _updateMapper.Map(weapon);

        patchDocument.ApplyTo(updateDto, ModelState);

        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }

        _updateMapper.Map(updateDto, weapon);
        await _weaponService.UpdateAsync(weapon);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _weaponService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("hit")]
    public async Task<ActionResult> Hit([FromBody] HitDto hitDto)
    {
        var dealer = await _characterService.GetByIdAsync(hitDto.DealerId);
        _playerService.VerifyPlayerAccessRights(dealer.Player!);

        var weapon = _characterService.GetWeapon(dealer, hitDto.ItemId);
        var receiver = await _characterService.GetByIdAsync(hitDto.ReceiverId);

        _characterService.CalculateHealth(receiver, weapon.Damage);
        await _characterService.UpdateAsync(receiver);

        return NoContent();
    }
}

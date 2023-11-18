using Core.Dtos;
using Core.Dtos.SpellDtos;
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

[Route("api/spells")]
[ApiController]
[Authorize]
[ApiVersion("1.0")]
[EnableRateLimiting("tokenBucket")]
public class SpellsController : ControllerBase
{
    private readonly IItemService<Spell> _spellService;
    private readonly ICharacterService _characterService;
    private readonly IPlayerService _playerService;
    private readonly IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>> _paginatedMapper;
    private readonly IMapper<Spell, SpellReadDto> _readMapper;
    private readonly IMapper<SpellBaseDto, Spell> _createMapper;
    private readonly IUpdateMapper<SpellBaseDto, Spell> _updateMapper;

    public SpellsController(
        IItemService<Spell> spellService,
        ICharacterService characterService,
        IPlayerService playerService,
        IMapper<PaginatedList<Spell>, PageDto<SpellReadDto>> paginatedMapper,
        IMapper<Spell, SpellReadDto> readMapper,
        IMapper<SpellBaseDto, Spell> createMapper,
        IUpdateMapper<SpellBaseDto, Spell> updateMapper)
    {
        _spellService = spellService;
        _characterService = characterService;
        _playerService = playerService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
        _createMapper = createMapper;
        _updateMapper = updateMapper;
    }

    [HttpGet]
    [CacheFilter]
    public async Task<ActionResult<PageDto<SpellReadDto>>> Get([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var spells = await _spellService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(spells);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<SpellReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var spell = await _spellService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(spell);

        return Ok(readDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SpellReadDto>> Create([FromBody] SpellBaseDto createDto)
    {
        var spell = _createMapper.Map(createDto);
        spell.Id = await _spellService.AddAsync(spell);

        var readDto = _readMapper.Map(spell);

        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] SpellBaseDto updateDto)
    {
        var spell = await _spellService.GetByIdAsync(id);

        _updateMapper.Map(updateDto, spell);
        await _spellService.UpdateAsync(spell);

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<SpellBaseDto> patchDocument)
    {
        var spell = await _spellService.GetByIdAsync(id);
        var updateDto = _updateMapper.Map(spell);

        patchDocument.ApplyTo(updateDto, ModelState);

        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }

        _updateMapper.Map(updateDto, spell);
        await _spellService.UpdateAsync(spell);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _spellService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("hit")]
    public async Task<ActionResult> Hit([FromBody] HitDto hitDto)
    {
        var dealer = await _characterService.GetByIdAsync(hitDto.DealerId);
        _playerService.VerifyPlayerAccessRights(dealer.Player!);

        var spell = _characterService.GetSpell(dealer, hitDto.ItemId);
        var receiver = await _characterService.GetByIdAsync(hitDto.ReceiverId);

        _characterService.CalculateHealth(receiver, spell.Damage);
        await _characterService.UpdateAsync(receiver);

        return NoContent();
    }
}

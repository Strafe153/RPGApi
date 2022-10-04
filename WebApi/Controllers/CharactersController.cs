using Core.Dtos;
using Core.Dtos.CharacterDtos;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers;

[Route("api/characters")]
[ApiController]
[Authorize]
public class CharactersController : ControllerBase
{
    private readonly ICharacterService _characterService;
    private readonly IPlayerService _playerService;
    private readonly IItemService<Weapon> _weaponService;
    private readonly IItemService<Spell> _spellService;
    private readonly IItemService<Mount> _mountService;
    private readonly IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>> _paginatedMapper;
    private readonly IMapper<Character, CharacterReadDto> _readMapper;
    private readonly IMapper<CharacterCreateDto, Character> _createMapper;
    private readonly IUpdateMapper<CharacterBaseDto, Character> _updateMapper;

    public CharactersController(
        ICharacterService characterService,
        IPlayerService playerService,
        IItemService<Weapon> weaponService,
        IItemService<Spell> spellService,
        IItemService<Mount> mountService,
        IMapper<PaginatedList<Character>, PageDto<CharacterReadDto>> paginatedMapper,
        IMapper<Character, CharacterReadDto> readMapper,
        IMapper<CharacterCreateDto, Character> createMapper,
        IUpdateMapper<CharacterBaseDto, Character> updateMapper)
    {
        _characterService = characterService;
        _playerService = playerService;
        _weaponService = weaponService;
        _spellService = spellService;
        _mountService = mountService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
        _createMapper = createMapper;
        _updateMapper = updateMapper;
    }

    [HttpGet]
    public async Task<ActionResult<PageDto<CharacterReadDto>>> GetAsync([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var characters = await _characterService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(characters);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<CharacterReadDto>> GetAsync([FromRoute] int id, CancellationToken token)
    {
        var character = await _characterService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(character);

        return Ok(readDto);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterReadDto>> CreateAsync([FromBody] CharacterCreateDto createDto)
    {
        var player = await _playerService.GetByIdAsync(createDto.PlayerId);
        _playerService.VerifyPlayerAccessRights(player);

        var character = _createMapper.Map(createDto);
        character.Id = await _characterService.AddAsync(character);

        var readDto = _readMapper.Map(character);

        return CreatedAtAction(nameof(GetAsync), new { id = readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] CharacterBaseDto updateDto)
    {
        var character = await _characterService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        _updateMapper.Map(updateDto, character);
        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<CharacterBaseDto> patchDocument)
    {
        var character = await _characterService.GetByIdAsync(id);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var updateDto = _updateMapper.Map(character);
        patchDocument.ApplyTo(updateDto, ModelState);

        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }

        _updateMapper.Map(updateDto, character);
        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var character = await _characterService.GetByIdAsync(id);

        _playerService.VerifyPlayerAccessRights(character.Player!);
        await _characterService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPut("add/weapon")]
    public async Task<ActionResult> AddWeaponAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var weapon = await _weaponService.GetByIdAsync(itemDto.ItemId);
        await _weaponService.AddToCharacterAsync(character, weapon);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPut("remove/weapon")]
    public async Task<ActionResult> RemoveWeaponAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var weapon = await _weaponService.GetByIdAsync(itemDto.ItemId);
        await _weaponService.RemoveFromCharacterAsync(character, weapon);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPut("add/spell")]
    public async Task<ActionResult> AddSpellAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var spell = await _spellService.GetByIdAsync(itemDto.ItemId);
        await _spellService.AddToCharacterAsync(character, spell);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPut("remove/spell")]
    public async Task<ActionResult> RemoveSpellAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var spell = await _spellService.GetByIdAsync(itemDto.ItemId);
        await _spellService.RemoveFromCharacterAsync(character, spell);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPut("add/mount")]
    public async Task<ActionResult> AddMountAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var mount = await _mountService.GetByIdAsync(itemDto.ItemId);
        await _mountService.AddToCharacterAsync(character, mount);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }

    [HttpPut("remove/mount")]
    public async Task<ActionResult> RemoveMountAsync([FromBody] AddRemoveItemDto itemDto)
    {
        var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
        _playerService.VerifyPlayerAccessRights(character.Player!);

        var mount = await _mountService.GetByIdAsync(itemDto.ItemId);
        await _mountService.RemoveFromCharacterAsync(character, mount);

        await _characterService.UpdateAsync(character);

        return NoContent();
    }
}

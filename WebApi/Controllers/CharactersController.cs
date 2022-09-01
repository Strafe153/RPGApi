using Core.Dtos;
using Core.Dtos.CharacterDtos;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
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
        public async Task<ActionResult<PageDto<CharacterReadDto>>> GetAsync([FromQuery] PageParameters pageParams)
        {
            var characters = await _characterService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var pageDto = _paginatedMapper.Map(characters);

            return Ok(pageDto);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<CharacterReadDto>> GetAsync([FromRoute] int id)
        {
            var character = await _characterService.GetByIdAsync(id);
            var readDto = _readMapper.Map(character);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadDto>> CreateAsync([FromBody] CharacterCreateDto createDto)
        {
            var player = await _playerService.GetByIdAsync(createDto.PlayerId);
            var character = _createMapper.Map(createDto);

            _playerService.VerifyPlayerAccessRights(player, User.Identity!, User.Claims!);
            await _characterService.AddAsync(character);

            var readDto = _readMapper.Map(character);

            return CreatedAtAction(nameof(GetAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] CharacterBaseDto updateDto)
        {
            var character = await _characterService.GetByIdAsync(id);

            _playerService.VerifyPlayerAccessRights(character.Player!, User.Identity!, User.Claims!);
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
            var updateDto = _updateMapper.Map(character);

            _playerService.VerifyPlayerAccessRights(character.Player!, User.Identity!, User.Claims!);
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

            _playerService.VerifyPlayerAccessRights(character.Player!, User.Identity!, User.Claims!);
            await _characterService.DeleteAsync(character);

            return NoContent();
        }

        [HttpPut("add/weapon")]
        public async Task<ActionResult> AddWeaponAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var weapon = await _weaponService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _weaponService.AddToCharacter(character, weapon);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/weapon")]
        public async Task<ActionResult> RemoveWeaponAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var weapon = await _weaponService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _weaponService.RemoveFromCharacter(character, weapon);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("add/spell")]
        public async Task<ActionResult> AddSpellAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var spell = await _spellService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _spellService.AddToCharacter(character, spell);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/spell")]
        public async Task<ActionResult> RemoveSpellAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var spell = await _spellService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _spellService.RemoveFromCharacter(character, spell);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("add/mount")]
        public async Task<ActionResult> AddMountAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var mount = await _mountService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _mountService.AddToCharacter(character, mount);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/mount")]
        public async Task<ActionResult> RemoveMountAsync([FromBody] AddRemoveItemDto itemDto)
        {
            var character = await _characterService.GetByIdAsync(itemDto.CharacterId);
            var mount = await _mountService.GetByIdAsync(itemDto.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _mountService.RemoveFromCharacter(character, mount);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }
    }
}

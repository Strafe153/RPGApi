using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.CharacterViewModels;
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
        private readonly IEnumerableMapper<PagedList<Character>, PageViewModel<CharacterReadViewModel>> _pagedMapper;
        private readonly IMapper<Character, CharacterReadViewModel> _readMapper;
        private readonly IMapper<CharacterCreateViewModel, Character> _createMapper;
        private readonly IUpdateMapper<CharacterBaseViewModel, Character> _updateMapper;

        public CharactersController(
            ICharacterService characterService,
            IPlayerService playerService,
            IItemService<Weapon> weaponService,
            IItemService<Spell> spellService,
            IItemService<Mount> mountService,
            IEnumerableMapper<PagedList<Character>, PageViewModel<CharacterReadViewModel>> pagedMapper,
            IMapper<Character, CharacterReadViewModel> readMapper,
            IMapper<CharacterCreateViewModel, Character> createMapper,
            IUpdateMapper<CharacterBaseViewModel, Character> updateMapper)
        {
            _characterService = characterService;
            _playerService = playerService;
            _weaponService = weaponService;
            _spellService = spellService;
            _mountService = mountService;
            _pagedMapper = pagedMapper;
            _readMapper = readMapper;
            _createMapper = createMapper;
            _updateMapper = updateMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageViewModel<CharacterReadViewModel>>> GetAsync(
            [FromQuery] PageParameters pageParams)
        {
            var characters = await _characterService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var readModels = _pagedMapper.Map(characters);

            return Ok(readModels);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<CharacterReadViewModel>> GetAsync([FromRoute] int id)
        {
            var character = await _characterService.GetByIdAsync(id);
            var readModel = _readMapper.Map(character);

            return Ok(readModel);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadViewModel>> CreateAsync(
            [FromBody] CharacterCreateViewModel createModel)
        {
            var player = await _playerService.GetByIdAsync(createModel.PlayerId);
            var character = _createMapper.Map(createModel);

            _playerService.VerifyPlayerAccessRights(player, User.Identity!, User.Claims!);
            await _characterService.AddAsync(character);

            var readModel = _readMapper.Map(character);

            return CreatedAtAction(nameof(GetAsync), new { id = readModel.Id }, readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id, 
            [FromBody] CharacterBaseViewModel updateModel)
        {
            var character = await _characterService.GetByIdAsync(id);

            _playerService.VerifyPlayerAccessRights(character.Player!, User.Identity!, User.Claims!);
            _updateMapper.Map(updateModel, character);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<CharacterBaseViewModel> patchDocument)
        {
            var character = await _characterService.GetByIdAsync(id);
            var updateModel = _updateMapper.Map(character);

            _playerService.VerifyPlayerAccessRights(character.Player!, User.Identity!, User.Claims!);
            patchDocument.ApplyTo(updateModel, ModelState);

            if (!TryValidateModel(updateModel))
            {
                return ValidationProblem(ModelState);
            }

            _updateMapper.Map(updateModel, character);
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
        public async Task<ActionResult> AddWeaponAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var weapon = await _weaponService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _weaponService.AddToCharacter(character, weapon);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/weapon")]
        public async Task<ActionResult> RemoveWeaponAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var weapon = await _weaponService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _weaponService.RemoveFromCharacter(character, weapon);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("add/spell")]
        public async Task<ActionResult> AddSpellAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var spell = await _spellService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _spellService.AddToCharacter(character, spell);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/spell")]
        public async Task<ActionResult> RemoveSpellAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var spell = await _spellService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _spellService.RemoveFromCharacter(character, spell);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("add/mount")]
        public async Task<ActionResult> AddMountAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var mount = await _mountService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _mountService.AddToCharacter(character, mount);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }

        [HttpPut("remove/mount")]
        public async Task<ActionResult> RemoveMountAsync([FromBody] AddRemoveItemViewModel itemModel)
        {
            var character = await _characterService.GetByIdAsync(itemModel.CharacterId);
            var mount = await _mountService.GetByIdAsync(itemModel.ItemId);

            _playerService.VerifyPlayerAccessRights(character.Player!, User?.Identity!, User?.Claims!);
            _mountService.RemoveFromCharacter(character, mount);
            await _characterService.UpdateAsync(character);

            return NoContent();
        }
    }
}

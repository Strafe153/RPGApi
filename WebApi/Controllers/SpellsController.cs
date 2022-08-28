using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.SpellViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/spells")]
    [ApiController]
    [Authorize]
    public class SpellsController : ControllerBase
    {
        private readonly IItemService<Spell> _spellService;
        private readonly ICharacterService _characterService;
        private readonly IPlayerService _playerService;
        private readonly IMapper<PaginatedList<Spell>, PageViewModel<SpellReadViewModel>> _paginatedMapper;
        private readonly IMapper<Spell, SpellReadViewModel> _readMapper;
        private readonly IMapper<SpellBaseViewModel, Spell> _createMapper;
        private readonly IUpdateMapper<SpellBaseViewModel, Spell> _updateMapper;

        public SpellsController(
            IItemService<Spell> spellService,
            ICharacterService characterService,
            IPlayerService playerService,
            IMapper<PaginatedList<Spell>, PageViewModel<SpellReadViewModel>> paginatedMapper,
            IMapper<Spell, SpellReadViewModel> readMapper,
            IMapper<SpellBaseViewModel, Spell> createMapper,
            IUpdateMapper<SpellBaseViewModel, Spell> updateMapper)
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
        public async Task<ActionResult<PageViewModel<SpellReadViewModel>>> GetAsync(
            [FromQuery] PageParameters pageParams)
        {
            var spells = await _spellService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var pageModel = _paginatedMapper.Map(spells);

            return Ok(pageModel);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<SpellReadViewModel>> GetAsync([FromRoute] int id)
        {
            var spell = await _spellService.GetByIdAsync(id);
            var readModel = _readMapper.Map(spell);

            return Ok(readModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpellReadViewModel>> CreateAsync([FromBody] SpellBaseViewModel createModel)
        {
            var spell = _createMapper.Map(createModel);
            await _spellService.AddAsync(spell);

            var readModel = _readMapper.Map(spell);

            return CreatedAtAction(nameof(GetAsync), new { Id = readModel.Id }, readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] SpellBaseViewModel updateModel)
        {
            var spell = await _spellService.GetByIdAsync(id);

            _updateMapper.Map(updateModel, spell);
            await _spellService.UpdateAsync(spell);

            return NoContent();
        }

        [HttpPatch("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<SpellBaseViewModel> patchDocument)
        {
            var spell = await _spellService.GetByIdAsync(id);
            var updateModel = _updateMapper.Map(spell);

            patchDocument.ApplyTo(updateModel, ModelState);

            if (!TryValidateModel(updateModel))
            {
                return ValidationProblem(ModelState);
            }

            _updateMapper.Map(updateModel, spell);
            await _spellService.UpdateAsync(spell);

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var spell = await _spellService.GetByIdAsync(id);
            await _spellService.DeleteAsync(spell);

            return NoContent();
        }

        [HttpPut("hit")]
        public async Task<ActionResult> HitAsync([FromBody] HitViewModel hitViewModel)
        {
            var dealer = await _characterService.GetByIdAsync(hitViewModel.DealerId);
            var spell = _characterService.GetSpell(dealer, hitViewModel.ItemId);
            var receiver = await _characterService.GetByIdAsync(hitViewModel.ReceiverId);

            _playerService.VerifyPlayerAccessRights(dealer.Player!, User.Identity!, User.Claims!);
            _characterService.CalculateHealth(receiver, spell.Damage);
            await _characterService.UpdateAsync(receiver);

            return NoContent();
        }
    }
}

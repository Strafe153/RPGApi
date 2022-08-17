using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.WeaponViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/weapons")]
    [ApiController]
    [Authorize]
    public class WeaponsController : ControllerBase
    {
        private readonly IItemService<Weapon> _weaponService;
        private readonly ICharacterService _characterService;
        private readonly IPlayerService _playerService;
        private readonly IEnumerableMapper<PagedList<Weapon>, PageViewModel<WeaponReadViewModel>> _pagedMapper;
        private readonly IMapper<Weapon, WeaponReadViewModel> _readMapper;
        private readonly IMapper<WeaponBaseViewModel, Weapon> _createMapper;
        private readonly IUpdateMapper<WeaponBaseViewModel, Weapon> _updateMapper;

        public WeaponsController(
            IItemService<Weapon> weaponService,
            ICharacterService characterService,
            IPlayerService playerService,
            IEnumerableMapper<PagedList<Weapon>, PageViewModel<WeaponReadViewModel>> pagedMapper,
            IMapper<Weapon, WeaponReadViewModel> readMapper,
            IMapper<WeaponBaseViewModel, Weapon> createMapper,
            IUpdateMapper<WeaponBaseViewModel, Weapon> updateMapper)
        {
            _weaponService = weaponService;
            _characterService = characterService;
            _playerService = playerService;
            _pagedMapper = pagedMapper;
            _readMapper = readMapper;
            _createMapper = createMapper;
            _updateMapper = updateMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageViewModel<WeaponReadViewModel>>> GetAsync(
            [FromQuery] PageParameters pageParams)
        {
            var weapons = await _weaponService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var readModels = _pagedMapper.Map(weapons);

            return Ok(readModels);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<WeaponReadViewModel>> GetAsync([FromRoute] int id)
        {
            var weapon = await _weaponService.GetByIdAsync(id);
            var readModel = _readMapper.Map(weapon);

            return Ok(readModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WeaponReadViewModel>> CreateAsync([FromBody] WeaponBaseViewModel createModel)
        {
            var weapon = _createMapper.Map(createModel);
            await _weaponService.AddAsync(weapon);

            var readModel = _readMapper.Map(weapon);

            return CreatedAtAction(nameof(GetAsync), new { Id = readModel.Id }, readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] WeaponBaseViewModel updateModel)
        {
            var weapon = await _weaponService.GetByIdAsync(id);

            _updateMapper.Map(updateModel, weapon);
            await _weaponService.UpdateAsync(weapon);

            return NoContent();
        }

        [HttpPatch("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<WeaponBaseViewModel> patchDocument)
        {
            var weapon = await _weaponService.GetByIdAsync(id);
            var updateModel = _updateMapper.Map(weapon);

            patchDocument.ApplyTo(updateModel, ModelState);

            if (!TryValidateModel(updateModel))
            {
                return ValidationProblem(ModelState);
            }

            _updateMapper.Map(updateModel, weapon);
            await _weaponService.UpdateAsync(weapon);

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var weapon = await _weaponService.GetByIdAsync(id);
            await _weaponService.DeleteAsync(weapon);

            return NoContent();
        }

        [HttpPut("hit")]
        public async Task<ActionResult> HitAsync([FromBody] HitViewModel hitViewModel)
        {
            var dealer = await _characterService.GetByIdAsync(hitViewModel.DealerId);
            var weapon = _characterService.GetWeapon(dealer, hitViewModel.ItemId);
            var receiver = await _characterService.GetByIdAsync(hitViewModel.ReceiverId);

            _playerService.VerifyPlayerAccessRights(dealer.Player!, User.Identity!, User.Claims!);
            _characterService.CalculateHealth(receiver, weapon.Damage);
            await _characterService.UpdateAsync(receiver);

            return NoContent();
        }
    }
}

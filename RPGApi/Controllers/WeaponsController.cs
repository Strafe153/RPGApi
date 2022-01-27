using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Dtos.Weapons;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("/api/weapons")]
    public class WeaponsController : ControllerBase
    {
        private readonly IControllerRepository<Weapon> _weaponRepo;
        private readonly IControllerRepository<Character> _charRepo;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public WeaponsController(IControllerRepository<Weapon> weaponRepo, 
            IControllerRepository<Character> charRepo, IMapper mapper)
        {
            _weaponRepo = weaponRepo;
            _charRepo = charRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WeaponReadDto>>> GetAllWeaponsAsync()
        {
            IEnumerable<Weapon> weapons = await _weaponRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<WeaponReadDto>>(weapons);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<WeaponReadDto>>> GetPaginatedWeaponsAsync(int page)
        {
            IEnumerable<Weapon> weapons = await _weaponRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<WeaponReadDto>>(weapons);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<WeaponReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<WeaponReadDto>> GetWeaponAsync(Guid id)
        {
            Weapon? weapon = await _weaponRepo.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound("Weapon not found");
            }

            var readDto = _mapper.Map<WeaponReadDto>(weapon);

            return Ok(readDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WeaponReadDto>> CreateWeaponAsync(WeaponCreateUpdateDto createDto)
        {
            Weapon weapon = _mapper.Map<Weapon>(createDto);

            _weaponRepo.Add(weapon);
            await _weaponRepo.SaveChangesAsync();

            var readDto = _mapper.Map<WeaponReadDto>(weapon);

            return CreatedAtAction(nameof(GetWeaponAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateWeaponAsync(Guid id, WeaponCreateUpdateDto updateDto)
        {
            Weapon? weapon = await _weaponRepo.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound("Weapon not found");
            }

            _mapper.Map(updateDto, weapon);
            _weaponRepo.Update(weapon);
            await _weaponRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PartialUpdateWeaponAsync(Guid id, 
            [FromBody]JsonPatchDocument<WeaponCreateUpdateDto> patchDoc)
        {
            Weapon? weapon = await _weaponRepo.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound("Weapon not found");
            }

            var updateDto = _mapper.Map<WeaponCreateUpdateDto>(weapon);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, weapon);
            _weaponRepo.Update(weapon);
            await _weaponRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteWeaponAsync(Guid id)
        {
            Weapon? weapon = await _weaponRepo.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound("Weapon not found");
            }

            _weaponRepo.Delete(weapon);
            await _weaponRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("hit")]
        [Authorize]
        public async Task<ActionResult> Hit(HitDto hitDto)
        {
            Character? dealer = await _charRepo.GetByIdAsync(hitDto.DealerId);

            if (dealer is null)
            {
                return NotFound("Damage dealer not found");
            }

            if (!CheckPlayerAccessRights(dealer))
            {
                return Forbid("Not enough rights");
            }

            if (dealer.Health == 0)
            {
                return Forbid($"Character {dealer.Name} is dead");
            }

            Weapon? weapon = dealer.Weapons?.SingleOrDefault(w => w.Id == hitDto.ItemId);

            if (weapon is null)
            {
                return NotFound("Weapon not found");
            }

            Character? receiver = await _charRepo.GetByIdAsync(hitDto.ReceiverId);

            if (receiver is null)
            {
                return NotFound("Damage receiver not found");
            }

            receiver.Health = receiver.Health < weapon.Damage ? 0 
                : receiver.Health - weapon.Damage;

            _charRepo.Update(receiver);
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        private bool CheckPlayerAccessRights(Character character)
        {
            if (character.Player?.Name != User?.Identity?.Name && User?.Claims.Where(
                c => c.Value == PlayerRole.Admin.ToString()).Count() == 0)
            {
                return false;
            }

            return true;
        }
    }
}

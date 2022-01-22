using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Dtos.Characters;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/characters")]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly IControllerRepository<Character> _charRepo;
        private readonly IControllerRepository<Weapon> _weaponRepo;
        private readonly IControllerRepository<Spell> _spellRepo;
        private readonly IControllerRepository<Mount> _mountRepo;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public CharactersController(IControllerRepository<Character> charRepo,
            IControllerRepository<Weapon> weaponRepo, IControllerRepository<Spell> spellRepo,
            IControllerRepository<Mount> mountRepo, IMapper mapper)
        {
            _charRepo = charRepo;
            _weaponRepo = weaponRepo;
            _spellRepo = spellRepo;
            _mountRepo = mountRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetAllCharactersAsync()
        {
            IEnumerable<Character> characters = await _charRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<CharacterReadDto>>(characters);
            _charRepo.LogInformation("Get all characters");

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<PageDto<CharacterReadDto>>> GetPaginatedCharactersAsync(int page)
        {
            IEnumerable<Character> characters = await _charRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<CharacterReadDto>>(characters);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<CharacterReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            _charRepo.LogInformation($"Get characters on page {page}");

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterReadDto>> GetCharacterAsync(Guid id)
        {
            Character? character = await _charRepo.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            _charRepo.LogInformation($"Get character {character.Name}");
            var readDto = _mapper.Map<CharacterReadDto>(character);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadDto>> CreateCharacterAsync(CharacterCreateDto createDto)
        {
            Character character = _mapper.Map<Character>(createDto);

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Character creation failed");
                return Forbid();
            }

            _charRepo.Add(character);
            _charRepo.LogInformation($"Created character {character.Name}");
            await _charRepo.SaveChangesAsync();

            var readDto = _mapper.Map<CharacterReadDto>(character);

            return CreatedAtAction(nameof(GetCharacterAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCharacterAsync(Guid id, CharacterUpdateDto updateDto)
        {
            Character? character = await _charRepo.GetByIdAsync(id);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Character update failed");
                return Forbid();
            }

            _mapper.Map(updateDto, character);
            _charRepo.LogInformation($"Updated character {character.Name}");
            _charRepo.Update(character);
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateCharacterAsync(Guid id,
            [FromBody]JsonPatchDocument<CharacterUpdateDto> patchDoc)
        {
            Character? character = await _charRepo.GetByIdAsync(id);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Character update");
                return Forbid();
            }

            var updateDto = _mapper.Map<CharacterUpdateDto>(character);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                _charRepo.LogInformation("Validation failed");
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, character);
            _charRepo.Update(character);
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCharacterAsync(Guid id)
        {
            Character? character = await _charRepo.GetByIdAsync(id);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Character deletion failed");
                return Forbid();
            }

            _charRepo.Delete(character);
            _charRepo.LogInformation($"Deleted character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("add/weapon")]
        public async Task<ActionResult> AddWeaponAsync(CharacterAddRemoveItem addDto)
        {
            Character? character = await _charRepo.GetByIdAsync(addDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Weapon adding failed");
                return Forbid();
            }

            Weapon? weapon = await _weaponRepo.GetByIdAsync(addDto.ItemId);

            if (weapon is null)
            {
                _charRepo.LogInformation("Weapon not found");
                return BadRequest();
            }

            character.Weapons!.Add(weapon);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Added weapon {weapon.Name} to character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("remove/weapon")]
        public async Task<ActionResult> RemoveWeaponAsync(CharacterAddRemoveItem weaponDto)
        {
            Character? character = await _charRepo.GetByIdAsync(weaponDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Weapon removal failed");
                return Forbid();
            }

            Weapon? weapon = await _weaponRepo.GetByIdAsync(weaponDto.ItemId);

            if (weapon is null)
            {
                _charRepo.LogInformation("Weapon not found");
                return BadRequest();
            }

            character.Weapons!.Remove(weapon);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Removed weapon {weapon.Name} from character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("add/spell")]
        public async Task<ActionResult> AddSpellAsync(CharacterAddRemoveItem spellDto)
        {
            Character? character = await _charRepo.GetByIdAsync(spellDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Spell adding failed");
                return Forbid();
            }

            Spell? spell = await _spellRepo.GetByIdAsync(spellDto.ItemId);

            if (spell is null)
            {
                _charRepo.LogInformation("Spell not found");
                return BadRequest();
            }

            character.Spells!.Add(spell);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Added spell {spell.Name} to character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("remove/spell")]
        public async Task<ActionResult> RemoveSpellAsync(CharacterAddRemoveItem spellDto)
        {
            Character? character = await _charRepo.GetByIdAsync(spellDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Spell removal failed");
                return Forbid();
            }

            Spell? spell = await _spellRepo.GetByIdAsync(spellDto.ItemId);

            if (spell is null)
            {
                _charRepo.LogInformation("Spell not found");
                return BadRequest();
            }

            character.Spells!.Remove(spell);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Removed spell {spell.Name} from character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("add/mount")]
        public async Task<ActionResult> AddMountAsync(CharacterAddRemoveItem mountDto)
        {
            Character? character = await _charRepo.GetByIdAsync(mountDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Mount adding failed");
                return Forbid();
            }

            Mount? mount = await _mountRepo.GetByIdAsync(mountDto.ItemId);

            if (mount is null)
            {
                _charRepo.LogInformation("Mount not found");
                return BadRequest();
            }

            character.Mounts!.Add(mount);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Added mount {mount.Name} to character {character.Name}");
            await _charRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("remove/mount")]
        public async Task<ActionResult> RemoveMountAsync(CharacterAddRemoveItem mountDto)
        {
            Character? character = await _charRepo.GetByIdAsync(mountDto.CharacterId);

            if (character is null)
            {
                _charRepo.LogInformation("Character not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(character))
            {
                _charRepo.LogInformation("Mount removal failed");
                return Forbid();
            }

            Mount? mount = await _mountRepo.GetByIdAsync(mountDto.ItemId);

            if (mount is null)
            {
                _charRepo.LogInformation("Mount not found");
                return BadRequest();
            }

            character.Mounts!.Remove(mount);
            _charRepo.Update(character);
            _charRepo.LogInformation($"Removed mount {mount.Name} from character {character.Name}");
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

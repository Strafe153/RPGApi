using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Dtos.Spells;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/spells")]
    public class SpellsController : ControllerBase
    {
        private readonly IControllerRepository<Spell> _spellRepo;
        private readonly IControllerRepository<Character> _charRepo;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public SpellsController(IControllerRepository<Spell> spellRepo,
            IControllerRepository<Character> charRepo, IMapper mapper)
        {
            _spellRepo = spellRepo;
            _charRepo = charRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SpellReadDto>>> GetAllSpellsAsync()
        {
            IEnumerable<Spell> spells = await _spellRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<SpellReadDto>>(spells);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<SpellReadDto>>> GetPaginatedSpellsAsync(int page)
        {
            IEnumerable<Spell> weapons = await _spellRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<SpellReadDto>>(weapons);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<SpellReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SpellReadDto>> GetSpellAsync(Guid id)
        {
            Spell? spell = await _spellRepo.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<SpellReadDto>(spell);

            return Ok(readDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpellReadDto>> CreateSpellAsync(SpellCreateUpdateDto createDto)
        {
            Spell spell = _mapper.Map<Spell>(createDto);

            _spellRepo.Add(spell);
            await _spellRepo.SaveChangesAsync();

            var readDto = _mapper.Map<SpellReadDto>(spell);

            return CreatedAtAction(nameof(GetSpellAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateSpellAsync(Guid id, SpellCreateUpdateDto updateDto)
        {
            Spell? spell = await _spellRepo.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, spell);
            _spellRepo.Update(spell);
            await _spellRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PartialUpdateSpellAsync(Guid id, 
            [FromBody]JsonPatchDocument<SpellCreateUpdateDto> patchDoc)
        {
            Spell? spell = await _spellRepo.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            var updateDto = _mapper.Map<SpellCreateUpdateDto>(spell);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, spell);
            _spellRepo.Update(spell);
            await _spellRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSpellAsync(Guid id)
        {
            Spell? spell = await _spellRepo.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            _spellRepo.Delete(spell);
            await _spellRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("hit")]
        [Authorize]
        public async Task<ActionResult> HitAsync(HitDto hitDto)
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
                return BadRequest($"Character {dealer.Name} is dead");
            }

            Spell? spell = dealer.Spells?.SingleOrDefault(s => s.Id == hitDto.ItemId);

            if (spell is null)
            {
                return NotFound("Spell not found");
            }

            Character? receiver = await _charRepo.GetByIdAsync(hitDto.ReceiverId);

            if (receiver is null)
            {
                return NotFound("Damage receiver not found");
            }

            Utility.CalculateHealth(receiver, spell.Damage);
            _charRepo.Update(receiver);
            await _spellRepo.SaveChangesAsync();

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

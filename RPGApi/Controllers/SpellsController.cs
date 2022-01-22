using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IControllerRepository<Spell> _repository;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public SpellsController(IControllerRepository<Spell> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SpellReadDto>>> GetAllSpellsAsync()
        {
            IEnumerable<Spell> spells = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<SpellReadDto>>(spells);
            _repository.LogInformation("Get all spells");

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<SpellReadDto>>> GetPaginatedSpellsAsync(int page)
        {
            IEnumerable<Spell> weapons = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<SpellReadDto>>(weapons);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<SpellReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            _repository.LogInformation($"Get spells on page {page}");

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SpellReadDto>> GetSpellAsync(Guid id)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _repository.LogInformation("Spell not found");
                return NotFound();
            }

            _repository.LogInformation($"Get spell {spell.Name}");
            var readDto = _mapper.Map<SpellReadDto>(spell);

            return Ok(readDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpellReadDto>> CreateSpellAsync(SpellCreateUpdateDto createDto)
        {
            Spell spell = _mapper.Map<Spell>(createDto);

            _repository.Add(spell);
            _repository.LogInformation($"Created spell {spell.Name}");
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<SpellReadDto>(spell);

            return CreatedAtAction(nameof(GetSpellAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateSpellAsync(Guid id, SpellCreateUpdateDto updateDto)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _repository.LogInformation("Spell not found");
                return NotFound();
            }

            _mapper.Map(updateDto, spell);
            _repository.Update(spell);
            _repository.LogInformation($"Updated spell {spell.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PartialUpdateSpellAsync(Guid id, 
            [FromBody]JsonPatchDocument<SpellCreateUpdateDto> patchDoc)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _repository.LogInformation("Spell not found");
                return NotFound();
            }

            var updateDto = _mapper.Map<SpellCreateUpdateDto>(spell);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                _repository.LogInformation("Spell validation failed");
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, spell);
            _repository.Update(spell);
            _repository.LogInformation($"Updated spell {spell.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteSpellAsync(Guid id)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _repository.LogInformation("Spell not found");
                return NotFound();
            }

            _repository.Delete(spell);
            _repository.LogInformation($"Deleted spell {spell.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

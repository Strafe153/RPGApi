using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using RPGApi.Dtos;
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

        public SpellsController(IControllerRepository<Spell> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpellReadDto>>> GetSpellsAsync()
        {
            IEnumerable<Spell> spells = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<SpellReadDto>>(spells);

            return Ok(readDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpellReadDto>> GetSpellAsync(Guid id)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<SpellReadDto>(spell);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<SpellReadDto>> CreateSpellAsync(SpellCreateUpdateDto createDto)
        {
            Spell spell = _mapper.Map<Spell>(createDto);

            _repository.Add(spell);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<SpellReadDto>(spell);

            return CreatedAtAction(nameof(GetSpellAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSpellAsync(Guid id, SpellCreateUpdateDto updateDto)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, spell);
            _repository.Update(spell);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateSpellAsync(Guid id, 
            JsonPatchDocument<SpellCreateUpdateDto> patchDoc)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

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
            _repository.Update(spell);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSpellAsync(Guid id)
        {
            Spell? spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                return NotFound();
            }

            _repository.Delete(spell);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

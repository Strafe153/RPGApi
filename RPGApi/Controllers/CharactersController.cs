using Microsoft.AspNetCore.Mvc;
using RPGApi.Dtos;
using RPGApi.Models;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class CharactersController : ControllerBase
    {
        private readonly IControllerRepository<Character> _repository;
        private readonly IMapper _mapper;

        public CharactersController(IControllerRepository<Character> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetCharactersAsync()
        {
            IEnumerable<Character> characters = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<CharacterReadDto>>(characters);

            return Ok(readDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterReadDto>> GetCharacterAsync(Guid id)
        {
            Character? character = await _repository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<CharacterReadDto>(character);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadDto>> CreateCharacterAsync(CharacterCreateUpdateDto createDto)
        {
            Character character = _mapper.Map<Character>(createDto);

            _repository.Add(character);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<CharacterReadDto>(character);

            return CreatedAtAction(nameof(GetCharacterAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCharacterAsync(Guid id, CharacterCreateUpdateDto updateDto)
        {
            Character? character = await _repository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, character);
            _repository.Update(character);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCharacterAsync(Guid id)
        {
            Character? character = await _repository.GetByIdAsync(id);

            if (character is null)
            {
                return NotFound();
            }

            _repository.Delete(character);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

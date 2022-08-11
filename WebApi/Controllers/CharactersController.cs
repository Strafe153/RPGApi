using Core.Entities;
using Core.Interfaces.Services;
using Core.VeiwModels.Character;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/characters")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly IService<Character> _service;
        private readonly IMapper<IEnumerable<Character>, IEnumerable<CharacterReadViewModel>> _readEnumerableMapper;
        private readonly IMapper<Character, CharacterReadViewModel> _readMapper;
        private readonly IMapper<CharacterCreateViewModel, Character> _createMapper;
        private readonly IMapperUpdater<CharacterUpdateViewModel, Character> _updateMapper;

        public CharactersController(
            IService<Character> service,
            IMapper<IEnumerable<Character>, IEnumerable<CharacterReadViewModel>> readEnumerableMapper,
            IMapper<Character, CharacterReadViewModel> readMapper,
            IMapper<CharacterCreateViewModel, Character> createMapper,
            IMapperUpdater<CharacterUpdateViewModel, Character> updateMapper)
        {
            _service = service;
            _readEnumerableMapper = readEnumerableMapper;
            _readMapper = readMapper;
            _createMapper = createMapper;
            _updateMapper = updateMapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterReadViewModel>>> GetAsync()
        {
            var characters = await _service.GetAllAsync();
            var readModels = _readEnumerableMapper.Map(characters);

            return Ok(readModels);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<CharacterReadViewModel>> GetAsync([FromRoute] int id)
        {
            var character = await _service.GetByIdAsync(id);
            var readModel = _readMapper.Map(character);

            return Ok(readModel);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterReadViewModel>> CreateAsync(
            [FromBody] CharacterCreateViewModel createModel)
        {
            var character = _createMapper.Map(createModel);
            await _service.CreateAsync(character);

            var readModel = _readMapper.Map(character);

            return CreatedAtAction(nameof(GetAsync), new { id = readModel.Id }, readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] CharacterUpdateViewModel updateModel)
        {
            var character = await _service.GetByIdAsync(id);

            _updateMapper.Map(updateModel, character);
            await _service.UpdateAsync(character);

            return NoContent();
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] JsonPatchDocument<CharacterUpdateViewModel> patchDocument)
        {
            var character = await _service.GetByIdAsync(id);
            var updateModel = _updateMapper.Map(character);

            patchDocument.ApplyTo(updateModel, ModelState);

            if (!TryValidateModel(updateModel))
            {
                return ValidationProblem(ModelState);
            }

            _updateMapper.Map(updateModel, character);
            await _service.UpdateAsync(character);

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var character = await _service.GetByIdAsync(id);
            await _service.DeleteAsync(character);

            return NoContent();
        }
    }
}

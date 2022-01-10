using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using RPGApi.Dtos;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IControllerRepository<Player> _repository;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public PlayersController(IControllerRepository<Player> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerReadDto>>> GetAllPlayersAsync()
        {
            IEnumerable<Player> players = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<PlayerReadDto>>(players);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<PageDto<PlayerReadDto>>> GetPaginatedPlayersAsync(int page = 1)
        {
            IEnumerable<Player> players = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<PlayerReadDto>>(players);

            var pagePlayers = readDtos.Skip((page - 1) * PageSize).Take(PageSize);

            PageDto<PlayerReadDto> pageDto = new()
            {
                Items = pagePlayers,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerReadDto>> GetPlayerAsync(Guid id)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerReadDto>> CreatePlayerAsync(PlayerCreateUpdateDto createDto)
        {
            Player player = _mapper.Map<Player>(createDto);

            _repository.Add(player);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return CreatedAtAction(nameof(GetPlayerAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePlayerAsync(Guid id, PlayerCreateUpdateDto updateDto)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdatePlayerAsync(Guid id,
            [FromBody]JsonPatchDocument<PlayerCreateUpdateDto> patchDoc)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            var updateDto = _mapper.Map<PlayerCreateUpdateDto>(player);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayerAsync(Guid id)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            _repository.Delete(player);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

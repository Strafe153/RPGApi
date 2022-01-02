using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Models;
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

        public PlayersController(IControllerRepository<Player> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerReadDto>>> GetPlayersAsync()
        {
            IEnumerable<Player> players = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<PlayerReadDto>>(players);

            return Ok(readDtos);
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
            Player player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlayerAsync(Guid id)
        {
            Player player = await _repository.GetByIdAsync(id);

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

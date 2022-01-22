using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Dtos.Players;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerControllerRepository _repository;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public PlayersController(IPlayerControllerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlayerReadDto>>> GetAllPlayersAsync()
        {
            IEnumerable<Player> players = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<PlayerReadDto>>(players);
            _repository.LogInformation("Get all players");

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<PlayerReadDto>>> GetPaginatedPlayersAsync(int page)
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

            _repository.LogInformation($"Get players on page {page}");

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PlayerReadDto>> GetPlayerAsync(Guid id)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            _repository.LogInformation($"Get player {player.Name}");
            var readDto = _mapper.Map<PlayerReadDto>(player);

            return Ok(readDto);
        }

        [HttpPost("register")]
        public async Task<ActionResult<PlayerReadDto>> RegisterPlayerAsync(PlayerAuthorizeDto registerDto)
        {
            if (await _repository.GetByNameAsync(registerDto.Name!) != null)
            {
                _repository.LogInformation("Player registration failed");
                return BadRequest("Player with the given name already exists");
            }

            Player player = new();

            _repository.CreatePasswordHash(registerDto.Password!, 
                out byte[] passwordHash, out byte[] passwordSalt);

            player.Name = registerDto.Name;
            player.Role = PlayerRole.Player;
            player.PasswordHash = passwordHash;
            player.PasswordSalt = passwordSalt;

            _repository.Add(player);
            _repository.LogInformation($"Registered player {player.Name}");
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return CreatedAtAction(nameof(GetPlayerAsync), new { Id = readDto.Id }, readDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<PlayerWithTokenReadDto>> LoginPlayerAsync(PlayerAuthorizeDto loginDto)
        {
            Player? player = await _repository.GetByNameAsync(loginDto.Name!);

            if (player is null)
            {
                _repository.LogInformation("Player not found");
                return NotFound();
            }

            if (player.Name != loginDto.Name)
            {
                _repository.LogInformation("Player does not exist");
                return BadRequest("Player does not exist");
            }
            
            if (!_repository.VerifyPasswordHash(loginDto.Password!, 
                player.PasswordHash!, player.PasswordSalt!))
            {
                _repository.LogInformation("Incorrect password");
                return BadRequest("Incorrect password");
            }

            string token = _repository.CreateToken(player);

            var returnDto = _mapper.Map<PlayerWithTokenReadDto>(player);
            returnDto = returnDto with { Token = token };
            _repository.LogInformation($"Player {player.Name} logged in");

            return Ok(returnDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdatePlayerAsync(Guid id, PlayerUpdateDto updateDto)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                _repository.LogInformation("Player not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                _repository.LogInformation("Player update failed");
                return Forbid();
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
            _repository.LogInformation($"Updated player {player.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> PartialUpdatePlayerAsync(Guid id,
            [FromBody]JsonPatchDocument<PlayerUpdateDto> patchDoc)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                _repository.LogInformation("Player not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                _repository.LogInformation("Player update failed");
                return Forbid();
            }

            var updateDto = _mapper.Map<PlayerUpdateDto>(player);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                _repository.LogInformation("Validation failed");
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
            _repository.LogInformation($"Updated player {player.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletePlayerAsync(Guid id)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                _repository.LogInformation("Player not found");
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                _repository.LogInformation("Player deletion failed");
                return Forbid();
            }

            _repository.Delete(player);
            _repository.LogInformation($"Deleted player {player.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("changeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRoleAsync(PlayerChangeRoleDto changeDto)
        {
            Player? player = await _repository.GetByIdAsync(changeDto.Id);

            if (player is null)
            {
                _repository.LogInformation("Player not found");
                return NotFound();
            }

            player.Role = changeDto.Role;
            _repository.Update(player);
            _repository.LogInformation($"Changed role for player {player.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        private bool CheckPlayerAccessRights(Player player)
        {
            if (player?.Name != User?.Identity?.Name && User?.Claims.Where(
                c => c.Value == PlayerRole.Admin.ToString()).Count() == 0)
            {
                return false;
            }

            return true;
        }
    }
}

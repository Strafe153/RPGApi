using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Data;
using RPGApi.Dtos;
using RPGApi.Dtos.Players;
using RPGApi.Repositories.Interfaces;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerControllerRepository _playerRepo;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public PlayersController(IPlayerControllerRepository playerRepo, IMapper mapper)
        {
            _playerRepo = playerRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PlayerReadDto>>> GetAllPlayersAsync()
        {
            IEnumerable<Player> players = await _playerRepo.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<PlayerReadDto>>(players);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<PlayerReadDto>>> GetPaginatedPlayersAsync(int page)
        {
            IEnumerable<Player> players = await _playerRepo.GetAllAsync();
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
        [Authorize]
        public async Task<ActionResult<PlayerReadDto>> GetPlayerAsync(Guid id)
        {
            Player? player = await _playerRepo.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return Ok(readDto);
        }

        [HttpPost("register")]
        public async Task<ActionResult<PlayerReadDto>> RegisterPlayerAsync(PlayerAuthorizeDto registerDto)
        {
            if (await _playerRepo.GetByNameAsync(registerDto.Name!) != null)
            {
                return BadRequest("Player with the given name already exists");
            }

            Player player = new();

            _playerRepo.CreatePasswordHash(registerDto.Password!, 
                out byte[] passwordHash, out byte[] passwordSalt);

            player.Name = registerDto.Name;
            player.Role = PlayerRole.Player;
            player.PasswordHash = passwordHash;
            player.PasswordSalt = passwordSalt;

            _playerRepo.Add(player);
            _playerRepo.LogInformation($"Registered player {player.Name}");
            await _playerRepo.SaveChangesAsync();

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return CreatedAtAction(nameof(GetPlayerAsync), new { Id = readDto.Id }, readDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<PlayerWithTokenReadDto>> LoginPlayerAsync(PlayerAuthorizeDto loginDto)
        {
            Player? player = await _playerRepo.GetByNameAsync(loginDto.Name!);

            if (player is null)
            {
                return NotFound();
            }

            if (player.Name != loginDto.Name)
            {
                return BadRequest("Player does not exist");
            }
            
            if (!_playerRepo.VerifyPasswordHash(loginDto.Password!, 
                player.PasswordHash!, player.PasswordSalt!))
            {
                return BadRequest("Incorrect password");
            }

            _playerRepo.LogInformation($"Player {player.Name} logged in");

            string token = _playerRepo.CreateToken(player);

            var returnDto = _mapper.Map<PlayerWithTokenReadDto>(player);
            returnDto = returnDto with { Token = token };

            return Ok(returnDto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletePlayerAsync(Guid id)
        {
            Player? player = await _playerRepo.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                return Forbid();
            }

            _playerRepo.Delete(player);
            _playerRepo.LogInformation($"Deleted player {player.Name}");
            await _playerRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("changeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRoleAsync(PlayerChangeRoleDto changeDto)
        {
            Player? player = await _playerRepo.GetByIdAsync(changeDto.Id);

            if (player is null)
            {
                return NotFound();
            }

            player.Role = changeDto.Role;
            _playerRepo.Update(player);
            _playerRepo.LogInformation($"Changed role for {player.Name} to {changeDto.Role}");
            await _playerRepo.SaveChangesAsync();

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

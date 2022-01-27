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

            var readDto = _mapper.Map<PlayerReadDto>(player);

            return Ok(readDto);
        }

        [HttpPost("register")]
        public async Task<ActionResult<PlayerReadDto>> RegisterPlayerAsync(PlayerAuthorizeDto registerDto)
        {
            if (await _repository.GetByNameAsync(registerDto.Name!) != null)
            {
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
                return NotFound();
            }

            if (player.Name != loginDto.Name)
            {
                return BadRequest("Player does not exist");
            }
            
            if (!_repository.VerifyPasswordHash(loginDto.Password!, 
                player.PasswordHash!, player.PasswordSalt!))
            {
                return BadRequest("Incorrect password");
            }

            string token = _repository.CreateToken(player);

            var returnDto = _mapper.Map<PlayerWithTokenReadDto>(player);
            returnDto = returnDto with { Token = token };

            return Ok(returnDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdatePlayerAsync(Guid id, PlayerUpdateDto updateDto)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                return Forbid();
            }

            _mapper.Map(updateDto, player);
            _repository.Update(player);
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
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                return Forbid();
            }

            var updateDto = _mapper.Map<PlayerUpdateDto>(player);
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
        [Authorize]
        public async Task<ActionResult> DeletePlayerAsync(Guid id)
        {
            Player? player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                return NotFound();
            }

            if (!CheckPlayerAccessRights(player))
            {
                return Forbid();
            }

            _repository.Delete(player);
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
                return NotFound();
            }

            player.Role = changeDto.Role;
            _repository.Update(player);
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

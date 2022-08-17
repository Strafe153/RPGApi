using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Core.ViewModels.PlayerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/players")]
    [ApiController]
    [Authorize]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IPasswordService _passwordService;
        private readonly IEnumerableMapper<PagedList<Player>, PageViewModel<PlayerReadViewModel>> _pagedMapper;
        private readonly IMapper<Player, PlayerReadViewModel> _readMapper;
        private readonly IMapper<Player, PlayerWithTokenReadViewModel> _readWithTokenMapper;

        public PlayersController(
            IPlayerService playerService,
            IPasswordService passwordService,
            IEnumerableMapper<PagedList<Player>, PageViewModel<PlayerReadViewModel>> pageMapper,
            IMapper<Player, PlayerReadViewModel> readMapper,
            IMapper<Player, PlayerWithTokenReadViewModel> readWithTokenMapper)
        {
            _playerService = playerService;
            _passwordService = passwordService;
            _pagedMapper = pageMapper;
            _readMapper = readMapper;
            _readWithTokenMapper = readWithTokenMapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageViewModel<PlayerReadViewModel>>> GetAsync(
            [FromQuery] PageParameters pageParams)
        {
            var players = await _playerService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
            var readModels = _pagedMapper.Map(players);

            return Ok(readModels);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<PlayerReadViewModel>> GetAsync([FromRoute] int id)
        {
            var player = await _playerService.GetByIdAsync(id);
            var readModel = _readMapper.Map(player);

            return Ok(readModel);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<PlayerReadViewModel>> RegisterAsync(
            [FromBody] PlayerAuthorizeViewModel authorizeModel)
        {
            await _playerService.VerifyNameUniqueness(authorizeModel.Name!);
            _passwordService.CreatePasswordHash(authorizeModel.Password!, out byte[] hash, out byte[] salt);

            Player player = _playerService.CreatePlayer(authorizeModel.Name!, hash, salt);
            await _playerService.AddAsync(player);

            var readModel = _readMapper.Map(player);

            return CreatedAtAction(nameof(GetAsync), new { Id = readModel.Id }, readModel);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<PlayerWithTokenReadViewModel>> LoginAsync(
            [FromBody] PlayerAuthorizeViewModel authorizeModel)
        {
            var player = await _playerService.GetByNameAsync(authorizeModel.Name!);
            _passwordService.VerifyPasswordHash(authorizeModel.Password!, player.PasswordHash!, player.PasswordSalt!);

            string token = _passwordService.CreateToken(player);
            var readModel = _readWithTokenMapper.Map(player) with { Token = token };

            return Ok(readModel);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] PlayerUpdateViewModel updateModel)
        {
            var player = await _playerService.GetByIdAsync(id);

            _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);
            await _playerService.VerifyNameUniqueness(updateModel.Value!);
            player.Name = updateModel.Value;
            await _playerService.UpdateAsync(player);

            return NoContent();
        }

        [HttpPut("{id:int:min(1)}/changePassword")]
        public async Task<ActionResult<string>> ChangePasswordAsync(
            [FromRoute] int id, 
            [FromBody] PlayerUpdateViewModel updateModel)
        {
            var player = await _playerService.GetByIdAsync(id);

            _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);
            _passwordService.CreatePasswordHash(updateModel.Value!, out byte[] hash, out byte[] salt);
            _playerService.ChangePasswordData(player, hash, salt);
            await _playerService.UpdateAsync(player);

            string token = _passwordService.CreateToken(player);

            return Ok(token);
        }

        [HttpPut("{id:int:min(1)}/changeRole")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeRoleAsync(
            [FromRoute] int id,
            [FromBody] PlayerChangeRoleViewModel changeRoleModel)
        {
            var player = await _playerService.GetByIdAsync(id);

            player.Role = changeRoleModel.Role;
            await _playerService.UpdateAsync(player);

            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] int id)
        {
            var player = await _playerService.GetByIdAsync(id);

            _playerService.VerifyPlayerAccessRights(player, User?.Identity!, User?.Claims!);
            await _playerService.DeleteAsync(player);

            return NoContent();
        }
    }
}

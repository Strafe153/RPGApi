using Core.Entities;
using Core.Interfaces.Services;
using Core.VeiwModels.WeaponViewModels;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/weapons")]
    [ApiController]
    public class WeaponsController : ControllerBase
    {
        private readonly IService<Weapon> _service;
        private readonly IEnumerableMapper<IEnumerable<Weapon>, IEnumerable<WeaponReadViewModel>> _readEnumerableMapper;
        private readonly IMapper<Weapon, WeaponReadViewModel> _readMapper;

        public WeaponsController(
            IService<Weapon> service,
            IEnumerableMapper<IEnumerable<Weapon>, IEnumerable<WeaponReadViewModel>> readEnumerableMapper,
            IMapper<Weapon, WeaponReadViewModel> readMapper)
        {
            _service = service;
            _readEnumerableMapper = readEnumerableMapper;
            _readMapper = readMapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeaponReadViewModel>>> GetAsync()
        {
            var weapons = await _service.GetAllAsync();
            var readModels = _readEnumerableMapper.Map(weapons);

            return Ok(readModels);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<WeaponReadViewModel>> GetAsync(int id)
        {
            var weapon = await _service.GetByIdAsync(id);
            var readModel = _readMapper.Map(weapon);

            return Ok(readModel);
        }
    }
}

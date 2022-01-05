using Microsoft.AspNetCore.Mvc;
using RPGApi.Dtos;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("/api/weapons")]
    public class WeaponsController : ControllerBase
    {
        private readonly IControllerRepository<Weapon> _repository;
        private readonly IMapper _mapper;

        public WeaponsController(IControllerRepository<Weapon> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeaponReadDto>>> GetWeaponsAsync()
        {
            IEnumerable<Weapon> weapons = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<WeaponReadDto>>(weapons);

            return Ok(readDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeaponReadDto>> GetWeaponAsync(Guid id)
        {
            Weapon? weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<WeaponReadDto>(weapon);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<WeaponReadDto>> CreateWeaponAsync(WeaponCreateUpdateDto createDto)
        {
            Weapon weapon = _mapper.Map<Weapon>(createDto);

            _repository.Add(weapon);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<WeaponReadDto>(weapon);

            return CreatedAtAction(nameof(GetWeaponAsync), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWeaponAsync(Guid id, WeaponCreateUpdateDto updateDto)
        {
            Weapon? weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, weapon);
            _repository.Update(weapon);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWeaponAsync(Guid id)
        {
            Weapon? weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound();
            }

            _repository.Delete(weapon);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

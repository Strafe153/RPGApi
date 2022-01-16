using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Dtos;
using RPGApi.Dtos.Weapons;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("/api/weapons")]
    [Authorize(Roles = "Admin")]
    public class WeaponsController : ControllerBase
    {
        private readonly IControllerRepository<Weapon> _repository;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public WeaponsController(IControllerRepository<Weapon> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeaponReadDto>>> GetAllWeaponsAsync()
        {
            IEnumerable<Weapon> weapons = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<WeaponReadDto>>(weapons);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        public async Task<ActionResult<PageDto<WeaponReadDto>>> GetPaginatedWeaponsAsync(int page)
        {
            IEnumerable<Weapon> weapons = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<WeaponReadDto>>(weapons);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<WeaponReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            return Ok(pageDto);
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

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateWeaponAsync(Guid id, 
            [FromBody]JsonPatchDocument<WeaponCreateUpdateDto> patchDoc)
        {
            Weapon? weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                return NotFound();
            }

            var updateDto = _mapper.Map<WeaponCreateUpdateDto>(weapon);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                return ValidationProblem(ModelState);
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

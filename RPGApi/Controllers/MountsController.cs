using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Dtos;
using RPGApi.Dtos.Mounts;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/mounts")]
    public class MountsController : ControllerBase
    {
        private readonly IControllerRepository<Mount> _repository;
        private readonly IMapper _mapper;
        private const int PageSize = 3;

        public MountsController(IControllerRepository<Mount> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MountReadDto>>> GetAllMountsAsync()
        {
            IEnumerable<Mount> mounts = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<MountReadDto>>(mounts);
            _repository.LogInformation("Get all mounts");

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
        [Authorize]
        public async Task<ActionResult<PageDto<MountReadDto>>> GetPaginatedMountsAsync(int page)
        {
            IEnumerable<Mount> weapons = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<MountReadDto>>(weapons);

            var pageItems = readDtos.Skip((page - 1) * PageSize).Take(PageSize);
            PageDto<MountReadDto> pageDto = new()
            {
                Items = pageItems,
                PagesCount = (int)Math.Ceiling((double)readDtos.Count() / PageSize),
                CurrentPage = page
            };

            _repository.LogInformation($"Get mounts on page {page}");

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MountReadDto>> GetMountAsync(Guid id)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                _repository.LogInformation("Mount not found");
                return NotFound();
            }

            _repository.LogInformation($"Get mount {mount.Name}");
            var readDto = _mapper.Map<MountReadDto>(mount);

            return Ok(readDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MountReadDto>> CreateMountAsync(MountCreateUpdateDto createDto)
        {
            Mount mount = _mapper.Map<Mount>(createDto);

            _repository.Add(mount);
            _repository.LogInformation($"Created mount {mount.Name}");
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<MountReadDto>(mount);

            return CreatedAtAction(nameof(GetMountAsync), new { Id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateMountAsync(Guid id, MountCreateUpdateDto updateDto)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                _repository.LogInformation("Mount not found");
                return NotFound();
            }

            _mapper.Map(updateDto, mount);
            _repository.Update(mount);
            _repository.LogInformation($"Updated mount {mount.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PartialUpdateMountAsync(Guid id,
            [FromBody]JsonPatchDocument<MountCreateUpdateDto> patchDoc)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                _repository.LogInformation("Mount not found");
                return NotFound();
            }

            var updateDto = _mapper.Map<MountCreateUpdateDto>(mount);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                _repository.LogInformation($"Mount validation failed");
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, mount);
            _repository.Update(mount);
            _repository.LogInformation($"Updated mount {mount.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMountAsync(Guid id)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                _repository.LogInformation("Mount not found");
                return NotFound();
            }

            _repository.Delete(mount);
            _repository.LogInformation($"Deleted mount {mount.Name}");
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

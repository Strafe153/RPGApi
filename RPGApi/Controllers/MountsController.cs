﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using RPGApi.Dtos;
using RPGApi.Repositories;
using AutoMapper;

namespace RPGApi.Controllers
{
    [ApiController]
    [Route("api/mounts")]
    [Authorize(Roles = "Admin")]
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
        public async Task<ActionResult<IEnumerable<MountReadDto>>> GetAllMountsAsync()
        {
            IEnumerable<Mount> mounts = await _repository.GetAllAsync();
            var readDtos = _mapper.Map<IEnumerable<MountReadDto>>(mounts);

            return Ok(readDtos);
        }

        [HttpGet("page/{page}")]
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

            return Ok(pageDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MountReadDto>> GetMountAsync(Guid id)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                return NotFound();
            }

            var readDto = _mapper.Map<MountReadDto>(mount);

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<MountReadDto>> CreateMountAsync(MountCreateUpdateDto createDto)
        {
            Mount mount = _mapper.Map<Mount>(createDto);

            _repository.Add(mount);
            await _repository.SaveChangesAsync();

            var readDto = _mapper.Map<MountReadDto>(mount);

            return CreatedAtAction(nameof(GetMountAsync), new { Id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMountAsync(Guid id, MountCreateUpdateDto updateDto)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                return NotFound();
            }

            _mapper.Map(updateDto, mount);
            _repository.Update(mount);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateMountAsync(Guid id,
            [FromBody]JsonPatchDocument<MountCreateUpdateDto> patchDoc)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                return NotFound();
            }

            var updateDto = _mapper.Map<MountCreateUpdateDto>(mount);
            patchDoc.ApplyTo(updateDto, ModelState);

            if (!TryValidateModel(updateDto))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(updateDto, mount);
            _repository.Update(mount);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMountAsync(Guid id)
        {
            Mount? mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                return NotFound();
            }

            _repository.Delete(mount);
            await _repository.SaveChangesAsync();

            return NoContent();
        }
    }
}

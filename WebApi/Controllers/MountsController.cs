using Core.Dtos;
using Core.Dtos.MountDtos;
using Core.Entities;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers;

[Route("api/mounts")]
[ApiController]
[Authorize]
public class MountsController : ControllerBase
{
    private readonly IItemService<Mount> _mountService;
    private readonly IMapper<PaginatedList<Mount>, PageDto<MountReadDto>> _paginatedMapper;
    private readonly IMapper<Mount, MountReadDto> _readMapper;
    private readonly IMapper<MountBaseDto, Mount> _createMapper;
    private readonly IUpdateMapper<MountBaseDto, Mount> _updateMapper;

    public MountsController(
        IItemService<Mount> mountService,
        IMapper<PaginatedList<Mount>, PageDto<MountReadDto>> paginatedMapper, 
        IMapper<Mount, MountReadDto> readMapper, 
        IMapper<MountBaseDto, Mount> createMapper,
        IUpdateMapper<MountBaseDto, Mount> updateMapper)
    {
        _mountService = mountService;
        _paginatedMapper = paginatedMapper;
        _readMapper = readMapper;
        _createMapper = createMapper;
        _updateMapper = updateMapper;
    }

    [HttpGet]
    public async Task<ActionResult<PageDto<MountReadDto>>> GetAsync([FromQuery] PageParameters pageParams)
    {
        var mounts = await _mountService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize);
        var pageDto = _paginatedMapper.Map(mounts);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<ActionResult<MountReadDto>> GetAsync([FromRoute] int id)
    {
        var mount = await _mountService.GetByIdAsync(id);
        var readDto = _readMapper.Map(mount);

        return Ok(readDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MountReadDto>> CreateAsync([FromBody] MountBaseDto createDto)
    {
        var mount = _createMapper.Map(createDto);
        await _mountService.AddAsync(mount);

        var readDto = _readMapper.Map(mount);

        return CreatedAtAction(nameof(GetAsync), new { Id = readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateAsync([FromRoute] int id, [FromBody] MountBaseDto updateDto)
    {
        var mount = await _mountService.GetByIdAsync(id);

        _updateMapper.Map(updateDto, mount);
        await _mountService.UpdateAsync(mount);

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<MountBaseDto> patchDocument)
    {
        var mount = await _mountService.GetByIdAsync(id);
        var updateDto = _updateMapper.Map(mount);

        patchDocument.ApplyTo(updateDto, ModelState);

        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }

        _updateMapper.Map(updateDto, mount);
        await _mountService.UpdateAsync(mount);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var mount = await _mountService.GetByIdAsync(id);
        await _mountService.DeleteAsync(mount);

        return NoContent();
    }
}

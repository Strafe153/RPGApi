using Asp.Versioning;
using Domain.Constants;
using Domain.Dtos;
using Domain.Dtos.MountDtos;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Shared;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;
using WebApi.Mappers.Interfaces;

namespace WebApi.Controllers.V1;

[Route("api/mounts")]
[ApiController]
[Authorize]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
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
    [CacheFilter]
    public async Task<ActionResult<PageDto<MountReadDto>>> Get([FromQuery] PageParameters pageParams, CancellationToken token)
    {
        var mounts = await _mountService.GetAllAsync(pageParams.PageNumber, pageParams.PageSize, token);
        var pageDto = _paginatedMapper.Map(mounts);

        return Ok(pageDto);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<MountReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var mount = await _mountService.GetByIdAsync(id, token);
        var readDto = _readMapper.Map(mount);

        return Ok(readDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MountReadDto>> Create([FromBody] MountBaseDto createDto)
    {
        var mount = _createMapper.Map(createDto);
        mount.Id = await _mountService.AddAsync(mount);

        var readDto = _readMapper.Map(mount);

        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] MountBaseDto updateDto)
    {
        var mount = await _mountService.GetByIdAsync(id);

        _updateMapper.Map(updateDto, mount);
        await _mountService.UpdateAsync(mount);

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(
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
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _mountService.DeleteAsync(id);
        return NoContent();
    }
}

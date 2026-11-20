using Application.Dtos;
using Application.Dtos.MountDtos;
using Application.Services.Abstractions;
using Application.Shared;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Constants;
using WebApi.Filters;

namespace WebApi.Controllers.V1;

[Route("api/mounts")]
[ApiController]
[Authorize(Roles = nameof(PlayerRole.Admin))]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MountsController(IMountsService mountsService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<PageDto<MountReadDto>>> Get([FromQuery] PageParameters pageParams)
    {
        var mounts = await mountsService.GetAllAsync(pageParams);
        return Ok(mounts);
    }

    [HttpGet("{id:int:min(1)}")]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<MountReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var mount = await mountsService.GetByIdAsync(id, token);
        return Ok(mount);
    }

    [HttpPost]
    public async Task<ActionResult<MountReadDto>> Create([FromBody] MountCreateDto createDto)
    {
        var readDto = await mountsService.AddAsync(createDto);
        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] MountUpdateDto updateDto, CancellationToken token)
    {
        await mountsService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<MountUpdateDto> patchDocument,
        CancellationToken token)
    {
        var patchResult = await mountsService.PatchAsync(id, patchDocument, TryValidateModel, token);
        return patchResult ? NoContent() : ValidationProblem(ModelState);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await mountsService.DeleteAsync(id, token);
        return NoContent();
    }
}

using Application.Dtos;
using Application.Dtos.WeaponDtos;
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

[Route("api/weapons")]
[ApiController]
[Authorize(Roles = nameof(PlayerRole.Admin))]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class WeaponsController(IWeaponsService weaponsService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<PageDto<WeaponReadDto>>> Get([FromQuery] PageParameters pageParams)
    {
        var weapons = await weaponsService.GetAllAsync(pageParams);
        return Ok(weapons);
    }

    [HttpGet("{id:int:min(1)}")]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<WeaponReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var weapon = await weaponsService.GetByIdAsync(id, token);
        return Ok(weapon);
    }

    [HttpPost]
    public async Task<ActionResult<WeaponReadDto>> Create([FromBody] WeaponCreateDto createDto)
    {
        var readDto = await weaponsService.AddAsync(createDto);
        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] WeaponUpdateDto updateDto,
        CancellationToken token)
    {
        await weaponsService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<WeaponUpdateDto> patchDocument,
        CancellationToken token)
    {
        var patchResult = await weaponsService.PatchAsync(id, patchDocument, TryValidateModel, token);
        return patchResult ? NoContent() : ValidationProblem(ModelState);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await weaponsService.DeleteAsync(id, token);
        return NoContent();
    }
}

using Application.Dtos;
using Application.Dtos.SpellDtos;
using Application.Services.Abstractions;
using Asp.Versioning;
using Domain.Constants;
using Domain.Enums;
using Domain.Shared;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;

namespace WebApi.Controllers.V1;

[Route("api/spells")]
[ApiController]
[Authorize(Roles = nameof(PlayerRole.Admin))]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class SpellsController : ControllerBase
{
    private readonly ISpellsService _spellService;

    public SpellsController(ISpellsService spellsService)
    {
        _spellService = spellsService;
    }

    [HttpGet]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<PageDto<SpellReadDto>>> Get(
        [FromQuery] PageParameters pageParams,
        CancellationToken token)
    {
        var spells = await _spellService.GetAllAsync(pageParams, token);
        return Ok(spells);
    }

    [HttpGet("{id:int:min(1)}")]
    [Authorize]
    [CacheFilter]
    public async Task<ActionResult<SpellReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var spell = await _spellService.GetByIdAsync(id, token);
        return Ok(spell);
    }

    [HttpPost]
    public async Task<ActionResult<SpellReadDto>> Create([FromBody] SpellCreateDto createDto)
    {
        var readDto = await _spellService.AddAsync(createDto);
        return CreatedAtAction(nameof(Get), new { readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] SpellUpdateDto updateDto, CancellationToken token)
    {
        await _spellService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<SpellUpdateDto> patchDocument,
        CancellationToken token)
    {
        var patchResult = await _spellService.PatchAsync(id, patchDocument, TryValidateModel, token);
        return patchResult ? NoContent() : ValidationProblem(ModelState);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await _spellService.DeleteAsync(id, token);
        return NoContent();
    }
}

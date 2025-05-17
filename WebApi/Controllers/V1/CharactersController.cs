using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Application.Services.Abstractions;
using Asp.Versioning;
using Domain.Constants;
using Domain.Shared;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Filters;

namespace WebApi.Controllers.V1;

[Route("api/characters")]
[ApiController]
[Authorize]
[ApiVersion(ApiVersioningConstants.V1)]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class CharactersController : ControllerBase
{
    private readonly ICharactersService _charactersService;

    public CharactersController(ICharactersService charactersService)
    {
        _charactersService = charactersService;
    }

    [HttpGet]
    [CacheFilter]
    public async Task<ActionResult<PageDto<CharacterReadDto>>> Get(
        [FromQuery] PageParameters pageParams,
        CancellationToken token)
    {
        var characters = await _charactersService.GetAllAsync(pageParams, token);
        return Ok(characters);
    }

    [HttpGet("{id:int:min(1)}")]
    [CacheFilter]
    public async Task<ActionResult<CharacterReadDto>> Get([FromRoute] int id, CancellationToken token)
    {
        var mount = await _charactersService.GetByIdAsync(id, token);
        return Ok(mount);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterReadDto>> Create(
        [FromBody] CharacterCreateDto createDto,
        CancellationToken token)
    {
        var readDto = await _charactersService.AddAsync(createDto, token);
        return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] CharacterUpdateDto updateDto,
        CancellationToken token)
    {
        await _charactersService.UpdateAsync(id, updateDto, token);
        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<ActionResult> Update(
        [FromRoute] int id,
        [FromBody] JsonPatchDocument<CharacterUpdateDto> patchDocument,
        CancellationToken token)
    {
        var patchResult = await _charactersService.PatchAsync(id, patchDocument, TryValidateModel, token);
        return patchResult ? NoContent() : ValidationProblem(ModelState);
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        await _charactersService.DeleteAsync(id, token);
        return NoContent();
    }

    [HttpPut("item")]
    public async Task<ActionResult> ManageItem([FromBody] ManageItemDto itemDto, CancellationToken token)
    {
        await _charactersService.ManageItemAsync(itemDto, token);
        return NoContent();
    }

    [HttpPut("hit")]
    public async Task<ActionResult> Hit([FromBody] HitDto hitDto, CancellationToken token)
    {
        await _charactersService.HitAsync(hitDto, token);
        return NoContent();
    }
}

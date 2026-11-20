using Application.Dtos;
using Application.Dtos.SpellDtos;
using Application.Extensions;
using Application.Mappings;
using Application.Services.Abstractions;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class SpellsService(
    IItemRepository<Spell> spellsRepository,
    ILogger<SpellsService> logger) : ISpellsService
{
    public async Task<SpellReadDto> AddAsync(SpellCreateDto createDto)
    {
        var spell = createDto.ToSpell();
        spell.Id = await spellsRepository.AddAsync(spell);

        logger.LogInformation("Succesfully created a spell");

        var readDto = spell.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await spellsRepository.GetByIdOrThrowAsync(id, logger, token);
        await spellsRepository.DeleteAsync(id);

        logger.LogInformation("Succesfully deleted a spell with id {Id}", id);
    }

    public async Task<PageDto<SpellReadDto>> GetAllAsync(PageParameters pageParameters)
    {
        var result = await spellsRepository.GetAllAsync(pageParameters.PageNumber, pageParameters.PageSize);
        var pageDto = result.ToPageDto(pageParameters);

        logger.LogInformation("Successfully retrieved all spells");

        return pageDto;
    }

    public async Task<SpellReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var spell = await spellsRepository.GetByIdOrThrowAsync(id, logger, token);
        logger.LogInformation("Successfully retrieved a spell with id {Id}", id);

        var readDto = spell.ToReadDto();

        return readDto;
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<SpellUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var spell = await spellsRepository.GetByIdOrThrowAsync(id, logger, token);
        var updateDto = spell.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(spell);
        await spellsRepository.UpdateAsync(spell);

        return true;
    }

    public async Task UpdateAsync(int id, SpellUpdateDto updateDto, CancellationToken token)
    {
        var spell = await spellsRepository.GetByIdOrThrowAsync(id, logger, token);
        updateDto.Update(spell);

        await spellsRepository.UpdateAsync(spell);
        logger.LogInformation("Successfully updated a spell with id {Id}", id);
    }
}

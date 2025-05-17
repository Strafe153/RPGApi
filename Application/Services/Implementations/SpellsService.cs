using Application.Dtos;
using Application.Dtos.SpellDtos;
using Application.Mappings;
using Application.Services.Abstractions;
using DataAccess.Extensions;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class SpellsService : ISpellsService
{
    private readonly IItemRepository<Spell> _spellsRepository;
    private readonly ILogger<SpellsService> _logger;

    public SpellsService(
        IItemRepository<Spell> spellsRepository,
        ILogger<SpellsService> logger)
    {
        _spellsRepository = spellsRepository;
        _logger = logger;
    }

    public async Task<SpellReadDto> AddAsync(SpellCreateDto createDto)
    {
        var spell = createDto.ToSpell();
        spell.Id = await _spellsRepository.AddAsync(spell);

        _logger.LogInformation("Succesfully created a spell");

        var readDto = spell.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await _spellsRepository.GetByIdOrThrowAsync(id, _logger, token);
        await _spellsRepository.DeleteAsync(id);

        _logger.LogInformation("Succesfully deleted a spell with id {Id}", id);
    }

    public async Task<PageDto<SpellReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
    {
        var pagedList = await _spellsRepository.GetAllAsync(pageParameters, token);
        _logger.LogInformation("Successfully retrieved all spells");

        var pageDto = pagedList.ToPageDto();

        return pageDto;
    }

    public async Task<SpellReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var spell = await _spellsRepository.GetByIdOrThrowAsync(id, _logger, token);
        _logger.LogInformation("Successfully retrieved a spell with id {Id}", id);

        var readDto = spell.ToReadDto();

        return readDto;
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<SpellUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var spell = await _spellsRepository.GetByIdOrThrowAsync(id, _logger, token);
        var updateDto = spell.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(spell);
        await _spellsRepository.UpdateAsync(spell);

        return true;
    }

    public async Task UpdateAsync(int id, SpellUpdateDto updateDto, CancellationToken token)
    {
        var spell = await _spellsRepository.GetByIdOrThrowAsync(id, _logger, token);
        updateDto.Update(spell);

        await _spellsRepository.UpdateAsync(spell);
        _logger.LogInformation("Successfully updated a spell with id {Id}", id);
    }
}

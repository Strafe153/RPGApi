using Application.Dtos;
using Application.Dtos.WeaponDtos;
using Application.Extensions;
using Application.Mappings;
using Application.Services.Abstractions;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class WeaponsService(
    IItemRepository<Weapon> repository,
    ILogger<WeaponsService> logger) : IWeaponsService
{
    public async Task<WeaponReadDto> AddAsync(WeaponCreateDto createDto)
    {
        var weapon = createDto.ToWeapon();
        weapon.Id = await repository.AddAsync(weapon);

        logger.LogInformation("Succesfully created a weapon");

        var readDto = weapon.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await repository.GetByIdOrThrowAsync(id, logger, token);
        await repository.DeleteAsync(id);

        logger.LogInformation("Succesfully deleted a weapon with id {Id}", id);
    }

    public async Task<PageDto<WeaponReadDto>> GetAllAsync(PageParameters pageParameters)
    {
        var result = await repository.GetAllAsync(pageParameters.PageNumber, pageParameters.PageSize);
        var pageDto = result.ToPageDto(pageParameters);

        logger.LogInformation("Successfully retrieved all weapons");

        return pageDto;
    }

    public async Task<WeaponReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var weapon = await repository.GetByIdOrThrowAsync(id, logger, token);
        logger.LogInformation("Successfully retrieved a weapon with id {Id}", id);

        var readDto = weapon.ToReadDto();

        return readDto;
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<WeaponUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var weapon = await repository.GetByIdOrThrowAsync(id, logger, token);
        var updateDto = weapon.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(weapon);
        await repository.UpdateAsync(weapon);

        return true;
    }

    public async Task UpdateAsync(int id, WeaponUpdateDto updateDto, CancellationToken token)
    {
        var weapon = await repository.GetByIdOrThrowAsync(id, logger, token);
        updateDto.Update(weapon);

        await repository.UpdateAsync(weapon);
        logger.LogInformation("Successfully updated a weapon with id {Id}", id);
    }
}

using Application.Dtos;
using Application.Dtos.MountDtos;
using Application.Extensions;
using Application.Mappings;
using Application.Services.Abstractions;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class MountsService(
    IItemRepository<Mount> repository,
    ILogger<MountsService> logger) : IMountsService
{
    public async Task<MountReadDto> AddAsync(MountCreateDto createDto)
    {
        var mount = createDto.ToMount();
        mount.Id = await repository.AddAsync(mount);
        
        logger.LogInformation("Succesfully created a mount");

        var readDto = mount.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await GetByIdAsync(id, token);
        await repository.DeleteAsync(id);

        logger.LogInformation("Succesfully deleted a mount with id {Id}", id);
    }

    public async Task<PageDto<MountReadDto>> GetAllAsync(PageParameters pageParameters)
    {
        var result = await repository.GetAllAsync(pageParameters.PageNumber, pageParameters.PageSize);
        var pageDto = result.ToPageDto(pageParameters);

        logger.LogInformation("Successfully retrieved all mounts");

        return pageDto;
    }

    public async Task<MountReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var mount = await repository.GetByIdOrThrowAsync(id, logger, token);
        logger.LogInformation("Successfully retrieved a mount with id {Id}", id);

        var readDto = mount.ToReadDto();

        return readDto;
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<MountUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var mount = await repository.GetByIdOrThrowAsync(id, logger, token);
        var updateDto = mount.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(mount);
        await repository.UpdateAsync(mount);

        return true;
    }

    public async Task UpdateAsync(int id, MountUpdateDto updateDto, CancellationToken token)
    {
        var mount = await repository.GetByIdOrThrowAsync(id, logger, token);
        updateDto.Update(mount);

        await repository.UpdateAsync(mount);
        logger.LogInformation("Successfully updated a mount with id {Id}", id);
    }
}

using Application.Dtos;
using Application.Dtos.MountDtos;
using Application.Mappings;
using Application.Services.Abstractions;
using DataAccess.Extensions;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class MountsService : IMountsService
{
    private readonly IItemRepository<Mount> _repository;
    private readonly ILogger<MountsService> _logger;

    public MountsService(
        IItemRepository<Mount> repository,
        ILogger<MountsService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MountReadDto> AddAsync(MountCreateDto createDto)
    {
        var mount = createDto.ToMount();
        mount.Id = await _repository.AddAsync(mount);
        _logger.LogInformation("Succesfully created a mount");

        var readDto = mount.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await GetByIdAsync(id, token);
        await _repository.DeleteAsync(id);

        _logger.LogInformation("Succesfully deleted a mount with id {Id}", id);
    }

    public async Task<PageDto<MountReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
    {
        var pagedList = await _repository.GetAllAsync(pageParameters, token);
        _logger.LogInformation("Successfully retrieved all mounts");

        var pageDto = pagedList.ToPageDto();

        return pageDto;
    }

    public async Task<MountReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var mount = await _repository.GetByIdOrThrowAsync(id, _logger, token);
        _logger.LogInformation("Successfully retrieved a mount with id {Id}", id);

        var readDto = mount.ToReadDto();

        return readDto;
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<MountUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var mount = await _repository.GetByIdOrThrowAsync(id, _logger, token);
        var updateDto = mount.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(mount);
        await _repository.UpdateAsync(mount);

        return true;
    }

    public async Task UpdateAsync(int id, MountUpdateDto updateDto, CancellationToken token)
    {
        var mount = await _repository.GetByIdOrThrowAsync(id, _logger, token);
        updateDto.Update(mount);

        await _repository.UpdateAsync(mount);
        _logger.LogInformation("Successfully updated a mount with id {Id}", id);
    }
}

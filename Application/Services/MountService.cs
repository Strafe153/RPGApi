using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class MountService : IItemService<Mount>
{
    private readonly IItemRepository<Mount> _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<MountService> _logger;

    public MountService(
        IItemRepository<Mount> repository,
        ICacheService cacheService,
        ILogger<MountService> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<int> AddAsync(Mount entity)
    {
        int id = await _repository.AddAsync(entity);
        _logger.LogInformation("Succesfully created a mount");

        return id;
    }

    public async Task AddToCharacterAsync(Character character, Mount item)
    {
        await _repository.AddToCharacterAsync(character, item);
        _logger.LogInformation("Successfully added the mount with id {ItemId} to the character with id {CharacterId}", 
            item.Id, character.Id);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Succesfully deleted a mount with id {Id}", id);
    }

    public async Task<PaginatedList<Mount>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        string key = $"mounts:{pageNumber}:{pageSize}";
        var cachedMounts = await _cacheService.GetAsync<List<Mount>>(key);
        PaginatedList<Mount> mounts;

        if (cachedMounts is null)
        {
            mounts = await _repository.GetAllAsync(pageNumber, pageSize, token);
            await _cacheService.SetAsync(key, mounts);
        }
        else
        {
            mounts = new(cachedMounts, cachedMounts.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all mounts");

        return mounts;
    }

    public async Task<Mount> GetByIdAsync(int id, CancellationToken token = default)
    {
        string key = $"mounts:{id}";
        var mount = await _cacheService.GetAsync<Mount>(key);

        if (mount is null)
        {
            mount = await _repository.GetByIdAsync(id, token);

            if (mount is null)
            {
                _logger.LogWarning("Failed to retrieve a mount with id {Id}", id);
                throw new NullReferenceException($"Mount with id {id} not found");
            }

            await _cacheService.SetAsync(key, mount);
        }

        _logger.LogInformation("Successfully retrieved a mount with id {Id}", id);

        return mount;
    }

    public async Task RemoveFromCharacterAsync(Character character, Mount item)
    {
        await _repository.RemoveFromCharacterAsync(character, item);
        _logger.LogInformation("Successfully removed the mount with id {ItemId} from the character with id {CharacterId}",
            item.Id, character.Id);
    }

    public async Task UpdateAsync(Mount entity)
    {
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Successfully updated a mount with id {Id}", entity.Id);
    }
}

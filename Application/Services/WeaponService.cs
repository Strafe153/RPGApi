﻿using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class WeaponService : IItemService<Weapon>
{
    private readonly IItemRepository<Weapon> _repository;
    private readonly ILogger<WeaponService> _logger;

    public WeaponService(
        IItemRepository<Weapon> repository,
        ILogger<WeaponService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> AddAsync(Weapon entity)
    {
        var id = await _repository.AddAsync(entity);
        _logger.LogInformation("Succesfully created a weapon");

        return id;
    }

    public async Task AddToCharacterAsync(Character character, Weapon item)
    {
        await _repository.AddToCharacterAsync(character, item);
        _logger.LogInformation("Successfully added the weapon with id {ItemId} to the character with id {CharacterId}",
            item.Id, character.Id);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Succesfully deleted a weapon with id {Id}", id);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var weapons = await _repository.GetAllAsync(pageNumber, pageSize, token);
        _logger.LogInformation("Successfully retrieved all weapons");

        return weapons;
    }

    public async Task<Weapon> GetByIdAsync(int id, CancellationToken token = default)
    {
        var weapon = await _repository.GetByIdAsync(id, token);

        if (weapon is null)
        {
            _logger.LogWarning("Failed to retrieve a weapon with id {Id}", id);
            throw new NullReferenceException($"Weapon with id {id} not found");
        }

        _logger.LogInformation("Successfully retrieved a weapon with id {Id}", id);

        return weapon;
    }

    public async Task RemoveFromCharacterAsync(Character character, Weapon item)
    {
        await _repository.RemoveFromCharacterAsync(character, item);
        _logger.LogInformation("Successfully removed the weapon with id {ItemId} from the character with id {CharacterId}",
            item.Id, character.Id);
    }

    public async Task UpdateAsync(Weapon entity)
    {
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Successfully updated a weapon with id {Id}", entity.Id);
    }
}

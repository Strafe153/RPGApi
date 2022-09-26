using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class WeaponService : IItemService<Weapon>
{
    private readonly IRepository<Weapon> _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<WeaponService> _logger;

    public WeaponService(
        IRepository<Weapon> repository,
        ICacheService cacheService,
        ILogger<WeaponService> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task AddAsync(Weapon entity)
    {
        _repository.Add(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succesfully created a weapon");
    }

    public void AddToCharacter(Character character, Weapon item)
    {
        CharacterWeapon characterWeapon = new()
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        character.CharacterWeapons.Add(characterWeapon);
        _logger.LogInformation("Successfully added the weapon with id {ItemId} to the character with id {CharacterId}",
            item.Id, character.Id);
    }

    public async Task DeleteAsync(Weapon entity)
    {
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succesfully deleted a weapon with id {Id}", entity.Id);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize)
    {
        string key = $"weapons:{pageNumber}:{pageSize}";
        var cachedWeapons = await _cacheService.GetAsync<List<Weapon>>(key);
        PaginatedList<Weapon> weapons;

        if (cachedWeapons is null)
        {
            weapons = await _repository.GetAllAsync(pageNumber, pageSize);
            await _cacheService.SetAsync(key, weapons);
        }
        else
        {
            weapons = new(cachedWeapons, cachedWeapons.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all weapons");

        return weapons;
    }

    public async Task<Weapon> GetByIdAsync(int id)
    {
        string key = $"weapons:{id}";
        var weapon = await _cacheService.GetAsync<Weapon>(key);

        if (weapon is null)
        {
            weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                _logger.LogWarning("Failed to retrieve a weapon with id {Id}", id);
                throw new NullReferenceException($"Weapon with id {id} not found");
            }

            await _cacheService.SetAsync(key, weapon);
        }

        _logger.LogInformation("Successfully retrieved a weapon with id {Id}", id);

        return weapon;
    }

    public void RemoveFromCharacter(Character character, Weapon item)
    {
        var characterWeapon = character.CharacterWeapons.SingleOrDefault(cw => cw.WeaponId == item.Id);

        if (characterWeapon is null)
        {
            _logger.LogWarning("Failed to remove the weapon with id {ItemId} from the character with id {CharacterId}", 
                item.Id, character.Id);
            throw new ItemNotFoundException($"Character with id {character.Id} does not possess the weapon with the id {item.Id}");
        }

        character.CharacterWeapons.Remove(characterWeapon);
        _logger.LogInformation("Successfully removed the weapon with id {ItemId} from the character with id {CharacterId}",
            item.Id, character.Id);
    }

    public async Task UpdateAsync(Weapon entity)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Successfully updated a weapon with id {Id}", entity.Id);
    }
}

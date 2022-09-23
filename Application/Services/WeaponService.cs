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
    private readonly ILogger<WeaponService> _logger;

    public WeaponService(
        IRepository<Weapon> repository,
        ILogger<WeaponService> logger)
    {
        _repository = repository;
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
        _logger.LogInformation($"Successfully added the weapon with id {item.Id} to the character with id {character.Id}");
    }

    public async Task DeleteAsync(Weapon entity)
    {
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation($"Succesfully deleted a weapon with id {entity.Id}");
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize)
    {
        var weapons = await _repository.GetAllAsync(pageNumber, pageSize);
        _logger.LogInformation("Successfully retrieved all weapons");

        return weapons;
    }

    public async Task<Weapon> GetByIdAsync(int id)
    {
        var weapon = await _repository.GetByIdAsync(id);

        if (weapon is null)
        {
            _logger.LogWarning($"Failed to retrieve a weapon with id {id}");
            throw new NullReferenceException("Weapon not found");
        }

        _logger.LogInformation($"Successfully retrieved a weapon with id {id}");

        return weapon;
    }

    public void RemoveFromCharacter(Character character, Weapon item)
    {
        var characterWeapon = character.CharacterWeapons.SingleOrDefault(cw => cw.WeaponId == item.Id);

        if (characterWeapon is null)
        {
            _logger.LogWarning($"Failed to remove the weapon with id {item.Id} from " +
                $"the character with id {character.Id}");
            throw new ItemNotFoundException($"Character with id {character.Id} does " +
                $"not possess the weapon with the id {item.Id}");
        }

        character.CharacterWeapons.Remove(characterWeapon);
        _logger.LogInformation($"Successfully removed the weapon with id {item.Id} " +
            $"from the character with id {character.Id}");
    }

    public async Task UpdateAsync(Weapon entity)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated a weapon with id {entity.Id}");
    }
}

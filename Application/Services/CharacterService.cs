using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IRepository<Character> _characterRepository;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(
        IRepository<Character> repository,
        ILogger<CharacterService> logger)
    {
        _characterRepository = repository;
        _logger = logger;
    }

    public async Task<int> AddAsync(Character entity)
    {
        var id = await _characterRepository.AddAsync(entity);
        _logger.LogInformation("Succesfully created a character");

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        await _characterRepository.DeleteAsync(id);
        _logger.LogInformation("Succesfully deleted a character with id {Id}", id);
    }

    public async Task<PaginatedList<Character>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var characters = await _characterRepository.GetAllAsync(pageNumber, pageSize, token);
        _logger.LogInformation("Successfully retrieved all characters");

        return characters;
    }

    public async Task<Character> GetByIdAsync(int id, CancellationToken token = default)
    {
        var character = await _characterRepository.GetByIdAsync(id, token);

        if (character is null)
        {
            _logger.LogWarning("Failed to retrieve a character with id {Id}", id);
            throw new NullReferenceException($"Character with id {id} not found");
        }

        _logger.LogInformation("Successfully retrieved a character with id {Id}", id);

        return character;
    }

    public async Task UpdateAsync(Character entity)
    {
        await _characterRepository.UpdateAsync(entity);
        _logger.LogInformation("Successfully updated a character with id {Id}", entity.Id);
    }

    public Weapon GetWeapon(Character entity, int weaponId)
    {
        var weapon = entity.CharacterWeapons
            .SingleOrDefault(cw => cw.WeaponId == weaponId)
            ?.Weapon;

        if (weapon is null)
        {
            _logger.LogWarning("Failed to retrieve a weapon with id {Id} from the user's inventory", weaponId);
            throw new ItemNotFoundException($"Weapon with id {weaponId} not found in the character's with id {entity.Id} inventory");
        }

        _logger.LogInformation("Successfully retrieved a spell with id {Id} from the character's inventory", weaponId);

        return weapon;
    }

    public void CalculateHealth(Character character, int damage)
    {
        if (character.Health - damage > 100)
        {
            character.Health = 100;
        }
        else if (character.Health < damage)
        {
            character.Health = 0;
        }
        else
        {
            character.Health -= damage;
        }
    }

    public Spell GetSpell(Character entity, int spellId)
    {
        var spell = entity.CharacterSpells
            .SingleOrDefault(cw => cw.SpellId == spellId)
            ?.Spell;

        if (spell is null)
        {
            _logger.LogWarning("Failed to retrieve a spell with id {Id} from the user's inventory", spellId);
            throw new ItemNotFoundException($"Spell with id {spellId} not found in the character's with id {entity.Id} inventory");
        }

        _logger.LogInformation("Successfully retrieved a spell with id {Id} from the character's inventory", spellId);

        return spell;
    }
}

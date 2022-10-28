using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CharacterService : ICharacterService
{
    private readonly IRepository<Character> _characterRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CharacterService> _logger;

    public CharacterService(
        IRepository<Character> repository,
        ICacheService cacheService,
        ILogger<CharacterService> logger)
    {
        _characterRepository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<int> AddAsync(Character entity)
    {
        int id = await _characterRepository.AddAsync(entity);
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
        string key = $"characters:{pageNumber}:{pageSize}";
        var cachedCharacters = await _cacheService.GetAsync<List<Character>>(key);
        PaginatedList<Character> characters;

        if (cachedCharacters is null)
        {
            characters = await _characterRepository.GetAllAsync(pageNumber, pageSize, token);
            await _cacheService.SetAsync(key, characters);
        }
        else
        {
            characters = new(cachedCharacters, cachedCharacters.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all characters");

        return characters;
    }

    public async Task<Character> GetByIdAsync(int id, CancellationToken token = default)
    {
        string key = $"characters:{id}";
        var character = await _cacheService.GetAsync<Character>(key);

        if (character is null)
        {
            character = await _characterRepository.GetByIdAsync(id, token);

            if (character is null)
            {
                _logger.LogWarning("Failed to retrieve a character with id {Id}", id);
                throw new NullReferenceException($"Character with id {id} not found");
            }

            await _cacheService.SetAsync(key, character);
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

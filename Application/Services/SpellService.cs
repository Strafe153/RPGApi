using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SpellService : IItemService<Spell>
{
    private readonly IRepository<Spell> _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<SpellService> _logger;

    public SpellService(
        IRepository<Spell> repository,
        ICacheService cacheService,
        ILogger<SpellService> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task AddAsync(Spell entity)
    {
        _repository.Add(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succesfully created a spell");
    }

    public void AddToCharacter(Character character, Spell item)
    {
        CharacterSpell characterSpell = new()
        {
            CharacterId = character.Id,
            SpellId = item.Id
        };

        character.CharacterSpells.Add(characterSpell);
        _logger.LogInformation("Successfully added the spell with id {ItemId} to the character with id {CharacterId}", 
            item.Id, character.Id);
    }

    public async Task DeleteAsync(Spell entity)
    {
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succesfully deleted a spell with id {Id}", entity.Id);
    }

    public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize)
    {
        string key = "spells";
        var cachedSpells = await _cacheService.GetAsync<List<Spell>>(key);
        PaginatedList<Spell> spells;

        if (cachedSpells is null)
        {
            spells = await _repository.GetAllAsync(pageNumber, pageSize);
            await _cacheService.SetAsync(key, spells);
        }
        else
        {
            spells = new(cachedSpells, cachedSpells.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all spells");

        return spells;
    }

    public async Task<Spell> GetByIdAsync(int id)
    {
        string key = $"spells:{id}";
        var spell = await _cacheService.GetAsync<Spell>(key);

        if (spell is null)
        {
            spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _logger.LogWarning("Failed to retrieve a spell with id {Id}", id);
                throw new NullReferenceException($"Spell with id {id} not found");
            }

            await _cacheService.SetAsync(key, spell);
        }

        _logger.LogInformation("Successfully retrieved a spell with id {Id}", id);

        return spell;
    }

    public void RemoveFromCharacter(Character character, Spell item)
    {
        var characterSpell = character.CharacterSpells.SingleOrDefault(cw => cw.SpellId == item.Id);

        if (characterSpell is null)
        {
            _logger.LogWarning("Failed to remove the spell with id {ItemId} from the character with id {CharacterId}",
                item.Id, character.Id);
            throw new ItemNotFoundException($"Character with id {character.Id} does not possess the spell with the id {item.Id}");
        }

        character.CharacterSpells.Remove(characterSpell);
        _logger.LogInformation("Successfully removed the spell with id {ItemId} from the character with id {CharacterId}", 
            item.Id, character.Id);
    }

    public async Task UpdateAsync(Spell entity)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Successfully updated a spell with id {Id}", entity.Id);
    }
}

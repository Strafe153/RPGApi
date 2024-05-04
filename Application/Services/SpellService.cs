using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SpellService : IItemService<Spell>
{
    private readonly IItemRepository<Spell> _repository;
    private readonly ILogger<SpellService> _logger;

    public SpellService(
        IItemRepository<Spell> repository,
        ILogger<SpellService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<int> AddAsync(Spell entity)
    {
        var id = await _repository.AddAsync(entity);
        _logger.LogInformation("Succesfully created a spell");

        return id;
    }

    public async Task AddToCharacterAsync(Character character, Spell item)
    {
        await _repository.AddToCharacterAsync(character, item);
        _logger.LogInformation("Successfully added the spell with id {ItemId} to the character with id {CharacterId}", 
            item.Id, character.Id);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Succesfully deleted a spell with id {Id}", id);
    }

    public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var spells = await _repository.GetAllAsync(pageNumber, pageSize, token);
        _logger.LogInformation("Successfully retrieved all spells");

        return spells;
    }

    public async Task<Spell> GetByIdAsync(int id, CancellationToken token = default)
    {
        var spell = await _repository.GetByIdAsync(id, token);

        if (spell is null)
        {
            _logger.LogWarning("Failed to retrieve a spell with id {Id}", id);
            throw new NullReferenceException($"Spell with id {id} not found");
        }

        _logger.LogInformation("Successfully retrieved a spell with id {Id}", id);

        return spell;
    }

    public async Task RemoveFromCharacterAsync(Character character, Spell item)
    {
        await _repository.RemoveFromCharacterAsync(character, item);
        _logger.LogInformation("Successfully removed the spell with id {ItemId} from the character with id {CharacterId}", 
            item.Id, character.Id);
    }

    public async Task UpdateAsync(Spell entity)
    {
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Successfully updated a spell with id {Id}", entity.Id);
    }
}

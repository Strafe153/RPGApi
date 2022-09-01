using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SpellService : IItemService<Spell>
    {
        private readonly IRepository<Spell> _repository;
        private readonly ILogger<SpellService> _logger;

        public SpellService(
            IRepository<Spell> repository,
            ILogger<SpellService> logger)
        {
            _repository = repository;
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
            _logger.LogInformation($"Successfully added the spell with id {item.Id} to the character with id {character.Id}");
        }

        public async Task DeleteAsync(Spell entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a spell with id {entity.Id}");
        }

        public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize)
        {
            var spells = await _repository.GetAllAsync(pageNumber, pageSize);
            _logger.LogInformation("Successfully retrieved all spells");

            return spells;
        }

        public async Task<Spell> GetByIdAsync(int id)
        {
            var spell = await _repository.GetByIdAsync(id);

            if (spell is null)
            {
                _logger.LogWarning($"Failed to retrieve a spell with id {id}");
                throw new NullReferenceException("Spell not found");
            }

            _logger.LogInformation($"Successfully retrieved a spell with id {id}");

            return spell;
        }

        public void RemoveFromCharacter(Character character, Spell item)
        {
            var characterSpell = character.CharacterSpells.SingleOrDefault(cw => cw.SpellId == item.Id);

            if (characterSpell is null)
            {
                _logger.LogWarning($"Failed to remove the spell with id {item.Id} from " +
                    $"the character with id {character.Id}");
                throw new ItemNotFoundException($"Character with id {character.Id} does " +
                    $"not possess the spell with the id {item.Id}");
            }

            character.CharacterSpells.Remove(characterSpell);
            _logger.LogInformation($"Successfully removed the spell with id {item.Id} " +
                $"from the character with id {character.Id}");
        }

        public async Task UpdateAsync(Spell entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a spell with id {entity.Id}");
        }
    }
}

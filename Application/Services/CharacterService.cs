using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly IRepository<Character> _characterRepository;
        private readonly ILogger _logger;

        public CharacterService(
            IRepository<Character> repository,
            ILogger<CharacterService> logger)
        {
            _characterRepository = repository;
            _logger = logger;
        }

        public async Task AddAsync(Character entity)
        {
            _characterRepository.Add(entity);
            await _characterRepository.SaveChangesAsync();

            _logger.LogInformation("Succesfully created a character");
        }

        public async Task DeleteAsync(Character entity)
        {
            _characterRepository.Delete(entity);
            await _characterRepository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a character with id {entity.Id}");
        }

        public async Task<PaginatedList<Character>> GetAllAsync(int pageNumber, int pageSize)
        {
            var characters = await _characterRepository.GetAllAsync(pageNumber, pageSize);
            _logger.LogInformation("Successfully retrieved all characters");

            return characters;
        }

        public async Task<Character> GetByIdAsync(int id)
        {
            var character = await _characterRepository.GetByIdAsync(id);

            if (character is null)
            {
                _logger.LogWarning($"Failed to retrieve a character with id {id}");
                throw new NullReferenceException("Character not found");
            }

            _logger.LogInformation($"Successfully retrieved a character with id {id}");

            return character;
        }

        public async Task UpdateAsync(Character entity)
        {
            _characterRepository.Update(entity);
            await _characterRepository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a character with id {entity.Id}");
        }

        public Weapon GetWeapon(Character entity, int weaponId)
        {
            var weapon = entity.CharacterWeapons
                .SingleOrDefault(cw => cw.WeaponId == weaponId)
                ?.Weapon;

            if (weapon is null)
            {
                _logger.LogWarning($"Failed to retrieve a spell with id {weaponId} from the user's inventory");
                throw new ItemNotFoundException("Weapon not found in the character's inventory");
            }

            _logger.LogInformation($"Successfully retrieved a spell with id {weaponId} from the character's inventory");

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
                _logger.LogWarning($"Failed to retrieve a spell with id {spellId} from the user's inventory");
                throw new ItemNotFoundException("Spell not found in the character's inventory");
            }

            _logger.LogInformation($"Successfully retrieved a spell with id {spellId} from the character's inventory");

            return spell;
        }
    }
}

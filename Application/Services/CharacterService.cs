using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CharacterService : IService<Character>
    {
        private readonly IRepository<Character> _repository;
        private readonly ILogger _logger;

        public CharacterService(
            IRepository<Character> repository,
            ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task CreateAsync(Character entity)
        {
            _repository.Create(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Succesfully created a character");
        }

        public async Task DeleteAsync(Character entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a character with id {entity.Id}");
        }

        public async Task<IEnumerable<Character>> GetAllAsync()
        {
            var characters = await _repository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved all characters");

            return characters;
        }

        public async Task<Character> GetByIdAsync(int id)
        {
            var character = await _repository.GetByIdAsync(id);

            if (character is null)
            {
                _logger.LogWarning($"Failed to retrieve a character with id {id}");
                throw new ArgumentNullException("Character not found");
            }

            _logger.LogInformation($"Successfully retrieved a character with id {id}");

            return character;
        }

        public async Task UpdateAsync(Character entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a character with id {entity.Id}");
        }
    }
}

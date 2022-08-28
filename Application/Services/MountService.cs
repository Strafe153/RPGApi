using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class MountService : IItemService<Mount>
    {
        private readonly IRepository<Mount> _repository;
        private readonly ILogger _logger;

        public MountService(
            IRepository<Mount> repository, 
            ILogger<MountService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddAsync(Mount entity)
        {
            _repository.Add(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Succesfully created a mount");
        }

        public void AddToCharacter(Character character, Mount item)
        {
            CharacterMount characterWeapon = new()
            {
                CharacterId = character.Id,
                MountId = item.Id
            };

            character.CharacterMounts.Add(characterWeapon);
            _logger.LogInformation($"Successfully added the mount with id {item.Id} to the character with id {character.Id}");
        }

        public async Task DeleteAsync(Mount entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a mount with id {entity.Id}");
        }

        public async Task<PaginatedList<Mount>> GetAllAsync(int pageNumber, int pageSize)
        {
            var mounts = await _repository.GetAllAsync(pageNumber, pageSize);
            _logger.LogInformation("Successfully retrieved all mounts");

            return mounts;
        }

        public async Task<Mount> GetByIdAsync(int id)
        {
            var mount = await _repository.GetByIdAsync(id);

            if (mount is null)
            {
                _logger.LogWarning($"Failed to retrieve a mount with id {id}");
                throw new NullReferenceException("Mount not found");
            }

            _logger.LogInformation($"Successfully retrieved a mount with id {id}");

            return mount;
        }

        public void RemoveFromCharacter(Character character, Mount item)
        {
            var characterMount = character.CharacterMounts.SingleOrDefault(cw => cw.MountId == item.Id);

            if (characterMount is null)
            {
                _logger.LogWarning($"Failed to remove the mount with id {item.Id} from " +
                    $"the character with id {character.Id}");
                throw new ItemNotFoundException($"Character with id {character.Id} does " +
                    $"not possess the mount with the id {item.Id}");
            }

            character.CharacterMounts.Remove(characterMount);
            _logger.LogInformation($"Successfully removed the mount with id {item.Id} " +
                $"from the character with id {character.Id}");
        }

        public async Task UpdateAsync(Mount entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a mount with id {entity.Id}");
        }
    }
}

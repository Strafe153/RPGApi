using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class WeaponService : IService<Weapon>
    {
        private readonly IRepository<Weapon> _repository;
        private readonly ILogger _logger;

        public WeaponService(
            IRepository<Weapon> repository,
            ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task CreateAsync(Weapon entity)
        {
            _repository.Create(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Succesfully created a weapon");
        }

        public async Task DeleteAsync(Weapon entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a weapon with id {entity.Id}");
        }

        public async Task<IEnumerable<Weapon>> GetAllAsync()
        {
            var weapons = await _repository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved all weapons");

            return weapons;
        }

        public async Task<Weapon> GetByIdAsync(int id)
        {
            var weapon = await _repository.GetByIdAsync(id);

            if (weapon is null)
            {
                _logger.LogWarning($"Failed to retrieve a weapon with id {id}");
                throw new ArgumentNullException("Weapon not found");
            }

            _logger.LogInformation($"Successfully retrieved a weapon with id {id}");

            return weapon;
        }

        public async Task UpdateAsync(Weapon entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a weapon with id {entity.Id}");
        }
    }
}

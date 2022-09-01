using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(
            IPlayerRepository repository,
            ILogger<PlayerService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddAsync(Player entity)
        {
            _repository.Add(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Succesfully registered a player");
        }

        public async Task DeleteAsync(Player entity)
        {
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Succesfully deleted a player with id {entity.Id}");
        }

        public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize)
        {
            var players = await _repository.GetAllAsync(pageNumber, pageSize);
            _logger.LogInformation("Successfully retrieved all players");

            return players;
        }

        public async Task<Player> GetByIdAsync(int id)
        {
            var player = await _repository.GetByIdAsync(id);

            if (player is null)
            {
                _logger.LogWarning($"Failed to retrieve a player with id {id}");
                throw new NullReferenceException("Player not found");
            }

            _logger.LogInformation($"Successfully retrieved a player with id {id}");

            return player;
        }

        public async Task<Player> GetByNameAsync(string name)
        {
            var player = await _repository.GetByNameAsync(name);

            if (player is null)
            {
                _logger.LogWarning($"Failed to retrieve a player with name '{name}'");
                throw new NullReferenceException("Player not found");
            }

            _logger.LogInformation($"Successfully retrieved a player with name '{name}'");

            return player;
        }

        public async Task VerifyNameUniqueness(string name)
        {
            var player = await _repository.GetByNameAsync(name);

            if (player is not null)
            {
                _logger.LogWarning($"Failed to create a player due to the name {name} being taken");
                throw new NameNotUniqueException($"Name '{name}' is already taken");
            }
        }

        public async Task UpdateAsync(Player entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated a player with id {entity.Id}");
        }

        public Player CreatePlayer(string name, byte[] passwordHash, byte[] passwordSalt)
        {
            Player player = new()
            {
                Name = name, 
                Role = PlayerRole.Player,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            return player;
        }

        public void ChangePasswordData(Player player, byte[] passwordHash, byte[] passwordSalt)
        {
            player.PasswordHash = passwordHash;
            player.PasswordSalt = passwordSalt;
        }

        public void VerifyPlayerAccessRights(Player performedOn, IIdentity performer, IEnumerable<Claim> claims)
        {
            if (performedOn.Name != performer.Name 
                && !claims.Any(c => c.Value == PlayerRole.Admin.ToString()))
            {
                throw new NotEnoughRightsException("Not enough rights to perform the operation");
            }
        }
    }
}

using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Security.Claims;

namespace Application.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository repository,
        ICacheService cacheService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PlayerService> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<int> AddAsync(Player entity)
    {
        try
        {
            int id = await _repository.AddAsync(entity);
            _logger.LogInformation("Succesfully registered a player");

            return id;
        }
        catch (NpgsqlException)
        {
            _logger.LogWarning("Failed to create a player. Name '{Name}' is already taken.", entity.Name);
            throw new NameNotUniqueException($"A player with name '{entity.Name}' already exists");
        }
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Succesfully deleted a player with id {Id}", id);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        string key = $"players:{pageNumber}:{pageSize}";
        var cachedPlayers = await _cacheService.GetAsync<List<Player>>(key);
        PaginatedList<Player> players;

        if (cachedPlayers is null)
        {
            players = await _repository.GetAllAsync(pageNumber, pageSize, token);
            await _cacheService.SetAsync(key, players);
        }
        else
        {
            players = new(cachedPlayers, cachedPlayers.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all players");

        return players;
    }

    public async Task<Player> GetByIdAsync(int id, CancellationToken token = default)
    {
        string key = $"players:{id}";
        var player = await _cacheService.GetAsync<Player>(key);

        if (player is null)
        {
            player = await _repository.GetByIdAsync(id, token);

            if (player is null)
            {
                _logger.LogWarning("Failed to retrieve a player with id {Id}", id);
                throw new NullReferenceException($"Player with id {id} not found");
            }

            await _cacheService.SetAsync(key, player);
        }

        _logger.LogInformation("Successfully retrieved a player with id {Id}", id);

        return player;
    }

    public async Task<Player> GetByNameAsync(string name, CancellationToken token = default)
    {
        string key = $"players:{name}";
        var player = await _cacheService.GetAsync<Player>(key);
        
        if (player is null)
        {
            player = await _repository.GetByNameAsync(name, token);

            if (player is null)
            {
                _logger.LogWarning("Failed to retrieve a player with name '{Name}'", name);
                throw new NullReferenceException($"Player with name '{name}' not found");
            }

            await _cacheService.SetAsync(key, player);
        }

        _logger.LogInformation("Successfully retrieved a player with name '{Name}'", name);

        return player;
    }

    public async Task UpdateAsync(Player entity)
    {
        try
        {
            await _repository.UpdateAsync(entity);
            _logger.LogInformation("Successfully updated a player with id {Id}", entity.Id);
        }
        catch (NpgsqlException)
        {
            _logger.LogWarning("Failed to update player with {Id}. Name '{Name}' is already taken.", entity.Id, entity.Name);
            throw new NameNotUniqueException($"A player with name '{entity.Name}' already exists");
        }
    }

    public Player CreatePlayer(string name, byte[] passwordHash, byte[] passwordSalt)
    {
        var player = new Player()
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

    public void VerifyPlayerAccessRights(Player performedOn)
    {
        var context = _httpContextAccessor.HttpContext;
        var claims = context.User.Claims;
        string performerName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
        string performerRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

        if ((performedOn.Name != performerName) && (performerRole != PlayerRole.Admin.ToString()))
        {
            throw new NotEnoughRightsException("Not enough rights to perform the operation");
        }
    }
}

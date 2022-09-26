using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Principal;

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

    public async Task AddAsync(Player entity)
    {
        try
        {
            _repository.Add(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Succesfully registered a player");
        }
        catch (DbUpdateException)
        {
            _logger.LogWarning("Failed to register a player. The name '{Name}' is already taken", entity.Name);
            throw new NameNotUniqueException($"Name '{entity.Name}' is already taken");
        }
    }

    public async Task DeleteAsync(Player entity)
    {
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Succesfully deleted a player with id {Id}", entity.Id);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize)
    {
        string key = $"players:{pageNumber}:{pageSize}";
        var cachedPlayers = await _cacheService.GetAsync<List<Player>>(key);
        PaginatedList<Player> players;

        if (cachedPlayers is null)
        {
            players = await _repository.GetAllAsync(pageNumber, pageSize);
            await _cacheService.SetAsync(key, players);
        }
        else
        {
            players = new(cachedPlayers, cachedPlayers.Count, pageNumber, pageSize);
        }

        _logger.LogInformation("Successfully retrieved all players");

        return players;
    }

    public async Task<Player> GetByIdAsync(int id)
    {
        string key = $"players:{id}";
        var player = await _cacheService.GetAsync<Player>(key);

        if (player is null)
        {
            player = await _repository.GetByIdAsync(id);

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

    public async Task<Player> GetByNameAsync(string name)
    {
        var player = await _repository.GetByNameAsync(name);

        if (player is null)
        {
            _logger.LogWarning("Failed to retrieve a player with name '{Name}'", name);
            throw new NullReferenceException($"Player with name {name} not found");
        }

        _logger.LogInformation("Successfully retrieved a player with name '{Name}'", name);

        return player;
    }

    public async Task UpdateAsync(Player entity)
    {
        try
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Successfully updated a player with id {Id}", entity.Id);
        }
        catch (DbUpdateException)
        {
            _logger.LogWarning("Failed to update a player. The name '{Name}' is already taken", entity.Name);
            throw new NameNotUniqueException($"Name '{entity.Name}' is already taken");
        }
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

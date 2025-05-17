using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Mappings;
using Application.Services.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Application.Services.Implementations;

public class PlayersService : IPlayersService
{
    private readonly IPlayersRepository _repository;
    private readonly IAccessHelper _accessHelper;
    private readonly ITokenHelper _tokenHelper;
    private readonly ILogger<PlayersService> _logger;

    public PlayersService(
        IPlayersRepository repository,
        IAccessHelper accessHelper,
        ITokenHelper tokenHelper,
        ILogger<PlayersService> logger)
    {
        _repository = repository;
        _tokenHelper = tokenHelper;
        _accessHelper = accessHelper;
        _logger = logger;
    }

    public async Task<PlayerReadDto> AddAsync(PlayerAuthorizeDto createDto)
    {
        try
        {
            var player = createDto.ToPlayer();

            (player.PasswordHash, player.PasswordSalt) = PasswordHelper.GeneratePasswordHashAndSalt(createDto.Password);
            player.Id = await _repository.AddAsync(player);

            _logger.LogInformation("Succesfully registered a player with name: {Name}", player.Name);

            var readDto = player.ToReadDto();

            return readDto;
        }
        catch (NpgsqlException)
        {
            _logger.LogWarning("Failed to create a player. Name '{Name}' is already taken.", createDto.Name);
            throw new NameNotUniqueException($"A player with name '{createDto.Name}' already exists");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _accessHelper.VerifyAccessRights(player);

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Succesfully deleted a player with id {Id}", id);
    }

    public async Task<PageDto<PlayerReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
    {
        var pagedList = await _repository.GetAllAsync(pageParameters, token);
        _logger.LogInformation("Successfully retrieved all players");

        var pageDto = pagedList.ToPageDto();

        return pageDto;
    }

    public async Task<PlayerReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _logger.LogInformation("Successfully retrieved a player with id {Id}", id);

        var readDto = player.ToReadDto();

        return readDto;
    }

    public async Task UpdateAsync(int id, PlayerUpdateDto updateDto, CancellationToken token)
    {
        try
        {
            var player = await GetByIdOrThrowAsync(id, token);
            _accessHelper.VerifyAccessRights(player);

            updateDto.Update(player);
            await _repository.UpdateAsync(player);

            _logger.LogInformation("Successfully updated a player with id {Id}", id);
        }
        catch (NpgsqlException)
        {
            _logger.LogWarning("Failed to update player with {Id}. Name '{Name}' is already taken.", id, updateDto.Name);
            throw new NameNotUniqueException($"A player with name '{updateDto.Name}' already exists");
        }
    }

    public async Task<TokensReadDto> LoginAsync(PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var player = await GetByNameAsync(authorizeDto.Name, token);
        PasswordHelper.VerifyPasswordHash(authorizeDto.Password, player);

        var accessToken = _tokenHelper.GenerateAccessToken(player);
        var refreshToken = _tokenHelper.GenerateRefreshToken();

        _tokenHelper.SetRefreshToken(player, refreshToken);
        await _repository.UpdateAsync(player);

        return new(accessToken, refreshToken);
    }

    public async Task<TokensReadDto> ChangePasswordAsync(int id, PlayerChangePasswordDto passwordDto, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _accessHelper.VerifyAccessRights(player);

        (player.PasswordHash, player.PasswordSalt) = PasswordHelper.GeneratePasswordHashAndSalt(passwordDto.Password);

        var accessToken = _tokenHelper.GenerateAccessToken(player);
        var refreshToken = _tokenHelper.GenerateRefreshToken();

        _tokenHelper.SetRefreshToken(player, refreshToken);
        await _repository.UpdateAsync(player);

        return new(accessToken, refreshToken);
    }

    public async Task<PlayerReadDto> ChangeRoleAsync(int id, PlayerChangeRoleDto roleDto, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        player.Role = roleDto.Role;

        await _repository.UpdateAsync(player);

        var readDto = player.ToReadDto();

        return readDto;
    }

    public async Task<TokensReadDto> RefreshTokensAsync(int id, TokensRefreshDto refreshDto, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _accessHelper.VerifyAccessRights(player);

        _tokenHelper.ValidateRefreshToken(player, refreshDto.RefreshToken);

        var accessToken = _tokenHelper.GenerateAccessToken(player);
        var refreshToken = _tokenHelper.GenerateRefreshToken();

        _tokenHelper.SetRefreshToken(player, refreshToken);
        await _repository.UpdateAsync(player);

        return new(accessToken, refreshToken);
    }

    private async Task<Player> GetByNameAsync(string name, CancellationToken token)
    {
        var player = await _repository.GetByNameAsync(name, token);

        if (player is null)
        {
            _logger.LogWarning("Failed to retrieve a player with name '{Name}'", name);
            throw new NullReferenceException($"Player with name '{name}' not found");
        }

        _logger.LogInformation("Successfully retrieved a player with name '{Name}'", name);

        return player;
    }

    private async Task<Player> GetByIdOrThrowAsync(int id, CancellationToken token)
    {
        var player = await _repository.GetByIdAsync(id, token);

        if (player is null)
        {
            _logger.LogWarning("Failed to retrieve a player with id {Id}", id);
            throw new NullReferenceException($"Player with id {id} not found");
        }

        return player;
    }
}

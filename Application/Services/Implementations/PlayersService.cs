using System.Data.Common;
using Application.Dtos;
using Application.Dtos.PlayerDtos;
using Application.Dtos.TokenDtos;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappings;
using Application.Services.Abstractions;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Security;
using Security.Interfaces;

namespace Application.Services.Implementations;

public class PlayersService : IPlayersService
{
    private readonly IPlayersRepository _repository;
    private readonly IAccessChecker _accesChecker;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<PlayersService> _logger;

    public PlayersService(
        IPlayersRepository repository,
        IAccessChecker accesChecker,
        ITokenGenerator tokenGenerator,
        IOptions<JwtOptions> jwtOptions,
        ILogger<PlayersService> logger)
    {
        _repository = repository;
        _accesChecker = accesChecker;
        _tokenGenerator = tokenGenerator;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<PlayerReadDto> AddAsync(PlayerAuthorizeDto createDto)
    {
        try
        {
            var player = createDto.ToPlayer();

            (player.PasswordHash, player.PasswordSalt) = PasswordHasher.GenerateHashWithSalt(createDto.Password);
            player.Id = await _repository.AddAsync(player);

            _logger.LogInformation("Succesfully registered a player with name: {Name}", player.Name);

            var readDto = player.ToReadDto();

            return readDto;
        }
        catch (DbException)
        {
            _logger.LogWarning("Failed to create a player. Name '{Name}' is already taken.", createDto.Name);
            throw new NameNotUniqueException($"A player with name '{createDto.Name}' already exists");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _accesChecker.VerifyAccessRights(player);

        await _repository.DeleteAsync(id);

        _logger.LogInformation("Succesfully deleted a player with id {Id}", id);
    }

    public async Task<PageDto<PlayerReadDto>> GetAllAsync(PageParameters pageParameters)
    {
        var result = await _repository.GetAllAsync(pageParameters.PageNumber, pageParameters.PageSize);
        var pageDto = result.ToPageDto(pageParameters);

        _logger.LogInformation("Successfully retrieved all players");

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
            _accesChecker.VerifyAccessRights(player);

            updateDto.Update(player);
            await _repository.UpdateAsync(player);

            _logger.LogInformation("Successfully updated a player with id {Id}", id);
        }
        catch (DbException)
        {
            _logger.LogWarning("Failed to update player with {Id}. Name '{Name}' is already taken.", id, updateDto.Name);
            throw new NameNotUniqueException($"A player with name '{updateDto.Name}' already exists");
        }
    }

    public async Task<TokensReadDto> LoginAsync(PlayerAuthorizeDto authorizeDto, CancellationToken token)
    {
        var player = await GetByNameAsync(authorizeDto.Name, token);
        PasswordHasher.Verify(authorizeDto.Password, player.PasswordSalt, player.PasswordHash);

        var accessToken = _tokenGenerator.GenerateAccessToken(player.GetClaims());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        SetRefreshToken(player, refreshToken);
        await _repository.UpdateAsync(player);

        return new(accessToken, refreshToken);
    }

    public async Task<TokensReadDto> ChangePasswordAsync(int id, PlayerChangePasswordDto passwordDto, CancellationToken token)
    {
        var player = await GetByIdOrThrowAsync(id, token);
        _accesChecker.VerifyAccessRights(player);

        (player.PasswordHash, player.PasswordSalt) = PasswordHasher.GenerateHashWithSalt(passwordDto.Password);

        var accessToken = _tokenGenerator.GenerateAccessToken(player.GetClaims());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        SetRefreshToken(player, refreshToken);
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
        _accesChecker.VerifyAccessRights(player);

        var validationResult = player.ValidateRefreshToken(refreshDto.RefreshToken);

        if (!validationResult)
        {
			_logger.LogWarning(
                "Player '{Name}' failed to validate a refresh token due to providing an incorrect or expired token",
                player.Name);

            throw new InvalidTokenException("Token is invalid or expired");
        }

        var accessToken = _tokenGenerator.GenerateAccessToken(player.GetClaims());
        var refreshToken = _tokenGenerator.GenerateRefreshToken();

        SetRefreshToken(player, refreshToken);
        await _repository.UpdateAsync(player);

        _logger.LogInformation("Player '{Name}' successfully refreshed the tokens", player.Name);

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

    private void SetRefreshToken(Player player, string token) =>
        player.SetRefreshToken(token, _jwtOptions.RefreshExpirationPeriod);
}

using Application.Mappers.Abstractions;
using Application.Services.Abstractions;
using Domain.Dtos;
using Domain.Dtos.PlayerDtos;
using Domain.Dtos.TokensDtos;
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
	private readonly IMapper<Player, PlayerReadDto, PlayerAuthorizeDto, PlayerUpdateDto> _mapper;
	private readonly ILogger<PlayersService> _logger;

	public PlayersService(
		IPlayersRepository repository,
		IAccessHelper accessHelper,
		ITokenHelper tokenHelper,
		IMapper<Player, PlayerReadDto, PlayerAuthorizeDto, PlayerUpdateDto> mapper,
		ILogger<PlayersService> logger)
	{
		_repository = repository;
		_tokenHelper = tokenHelper;
		_mapper = mapper;
		_accessHelper = accessHelper;
		_logger = logger;
	}

	public async Task<PlayerReadDto> AddAsync(PlayerAuthorizeDto createDto)
	{
		try
		{
			var player = _mapper.Map(createDto);

			(player.PasswordHash, player.PasswordSalt) = PasswordHelper.GeneratePasswordHashAndSalt(createDto.Password);
			player.Id = await _repository.AddAsync(player);

			_logger.LogInformation("Succesfully registered a player with name: {Name}", player.Name);

			return _mapper.Map(player);
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
		var players = await _repository.GetAllAsync(pageParameters, token);
		_logger.LogInformation("Successfully retrieved all players");

		return _mapper.Map(players);
	}

	public async Task<PlayerReadDto> GetByIdAsync(int id, CancellationToken token)
	{
		var player = await GetByIdOrThrowAsync(id, token);
		_logger.LogInformation("Successfully retrieved a player with id {Id}", id);

		return _mapper.Map(player);
	}

	public async Task UpdateAsync(int id, PlayerUpdateDto updateDto, CancellationToken token)
	{
		try
		{
			var player = await GetByIdOrThrowAsync(id, token);
			_accessHelper.VerifyAccessRights(player);

			_mapper.Map(updateDto, player);
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

		return _mapper.Map(player);
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

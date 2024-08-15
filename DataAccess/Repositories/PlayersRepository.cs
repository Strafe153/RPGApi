using Dapper;
using DataAccess.Database;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PlayersRepository : IPlayersRepository
{
	private readonly DatabaseConnectionProvider _connectionProvider;

	public PlayersRepository(DatabaseConnectionProvider connectionProvider)
	{
		_connectionProvider = connectionProvider;
	}

	public async Task<int> AddAsync(Player entity)
	{
		var queryParams = new
		{
			entity.Name,
			Role = PlayerRole.Player,
			entity.PasswordHash,
			entity.PasswordSalt
		};

		var query = @"
            INSERT INTO Players (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
            VALUES (@Name, @Role, @PasswordHash, @PasswordSalt)
            RETURNING ""Id""";

		using var connection = _connectionProvider.CreateConnection();
		var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

		return id;
	}

	public async Task DeleteAsync(int id)
	{
		var queryParams = new { Id = id };
		var query = @"
            DELETE FROM Players
            WHERE ""Id"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	public async Task<PagedList<Player>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
	{
		var queryParams = new
		{
			pageParameters.PageNumber,
			pageParameters.PageSize
		};

		var query = @"
            SELECT p.*,
                   c.*
            FROM Players AS p
            LEFT JOIN Characters AS c on p.""Id"" = c.""PlayerId""
            ORDER BY p.""Id"" ASC
            OFFSET @PageSize * (@PageNumber - 1)
            LIMIT @PageSize";

		using var connection = _connectionProvider.CreateConnection();
		var queryResult = await connection.QueryAsync<Player, Character, Player>(
			new CommandDefinition(query, queryParams, cancellationToken: token),
			(player, character) =>
			{
				player.Characters.Add(character);
				return player;
			},
			splitOn: "Id");

		var pagedList = queryResult
			.GroupBy(p => p.Id)
			.Select(g =>
			{
				var player = g.First();
				player.Characters = g.Select(p => p.Characters.First()).ToList();

				return player;
			})
			.ToPagedList(pageParameters);

		return pagedList;
	}

	public async Task<Player?> GetByIdAsync(int id, CancellationToken token)
	{
		var queryParams = new { Id = id };
		var query = @"
            SELECT p.*,
                   c.*
            FROM Players AS p
            LEFT JOIN Characters AS c on p.""Id"" = c.""PlayerId""
            WHERE p.""Id"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		var queryResult = await connection.QueryAsync<Player, Character, Player>(
			new CommandDefinition(query, queryParams, cancellationToken: token),
			(player, character) =>
			{
				player.Characters.Add(character);
				return player;
			});

		var player = queryResult
			.GroupBy(p => p.Id)
			.Select(g =>
			{
				var player = g.First();
				player.Characters = g.Select(p => p.Characters.First()).ToList();

				return player;
			})
			.FirstOrDefault();

		return player;
	}

	public async Task<Player?> GetByNameAsync(string name, CancellationToken token)
	{
		var queryParams = new { Name = name };
		var query = @"
            SELECT *
            FROM Players
            WHERE ""Name"" = @Name";

		using var connection = _connectionProvider.CreateConnection();
		var player = await connection.QueryFirstOrDefaultAsync<Player>(new CommandDefinition(query, queryParams, cancellationToken: token));

		return player;
	}

	public async Task UpdateAsync(Player entity)
	{
		var queryParams = new
		{
			entity.Id,
			entity.Name,
			entity.Role,
			entity.PasswordHash,
			entity.PasswordSalt,
			entity.RefreshToken,
			entity.RefreshTokenExpiryDate
		};

		var query = @"
            UPDATE Players
            SET ""Name"" = @Name, ""Role"" = @Role,
                ""PasswordHash"" = @PasswordHash,
                ""PasswordSalt"" = @PasswordSalt,
                ""RefreshToken"" = @RefreshToken,
                ""RefreshTokenExpiryDate"" = @RefreshTokenExpiryDate
            WHERE ""Id"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}
}

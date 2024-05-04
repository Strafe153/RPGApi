using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Shared;
using Dapper;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly RPGContext _context;

    public PlayerRepository(RPGContext context)
    {
        _context = context;
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
            INSERT INTO ""Players""
                (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
            VALUES
                (@Name, @Role, @PasswordHash, @PasswordSalt)
            RETURNING ""Id""";

        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public Task DeleteAsync(int id)
    {
        var queryParams = new { Id = id };
        var query = @"
            DELETE FROM ""Players"" 
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var queryParams = new
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var query = @"
            SELECT p.*,
                   c.*
            FROM ""Players"" AS p
            LEFT JOIN ""Characters"" AS c on p.""Id"" = c.""PlayerId""
            ORDER BY p.""Id"" ASC
            OFFSET @PageSize * (@PageNumber - 1)
            LIMIT @PageSize";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Player, Character, Player>(
            new CommandDefinition(query, queryParams, cancellationToken: token), 
            (player, character) =>
            {
                player.Characters.Add(character);
                return player;
            },
            splitOn: "Id");

        var paginatedList = queryResult
            .GroupBy(p => p.Id)
            .Select(g =>
            {
                var player = g.First();
                player.Characters = g.Select(p => p.Characters.First()).ToList();

                return player;
            })
            .ToPaginatedList(pageNumber, pageSize);


        return paginatedList;
    }

    public async Task<Player?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var queryParams = new { Id = id };
        var query = @"
            SELECT p.*,
                   c.*
            FROM ""Players"" AS p
            LEFT JOIN ""Characters"" AS c on p.""Id"" = c.""PlayerId""
            WHERE p.""Id"" = @Id";

        using var connection = _context.CreateConnection();
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

    public async Task<Player?> GetByNameAsync(string name, CancellationToken token = default)
    {
        var queryParams = new { Name = name };
        var query = @"
            SELECT *
            FROM ""Players""
            WHERE ""Name"" = @Name";

        using var connection = _context.CreateConnection();
        var player = await connection.QueryFirstOrDefaultAsync<Player>(new CommandDefinition(query, queryParams, cancellationToken: token));

        return player;
    }

    public Task UpdateAsync(Player entity)
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
            UPDATE ""Players"" 
            SET ""Name"" = @Name, ""Role"" = @Role,
                ""PasswordHash"" = @PasswordHash,
                ""PasswordSalt"" = @PasswordSalt,
                ""RefreshToken"" = @RefreshToken,
                ""RefreshTokenExpiryDate"" = @RefreshTokenExpiryDate
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }
}

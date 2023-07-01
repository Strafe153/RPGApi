using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Shared;
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
        var query = @"INSERT INTO ""Players"" (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
                      VALUES (@Name, @Role, @PasswordHash, @PasswordSalt)
                      RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Role = PlayerRole.Player,
            PasswordHash = entity.PasswordHash,
            PasswordSalt = entity.PasswordSalt
        };

        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        var query = @"DELETE FROM ""Players"" 
                      WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var query = @"SELECT *
                      FROM ""Players"" AS p
                      LEFT JOIN ""Characters"" AS c on p.""Id"" = c.""PlayerId""
                      ORDER BY p.""Id"" ASC";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Player, Character, Player>(
            new CommandDefinition(query, cancellationToken: token), 
            (player, character) =>
            {
                player.Characters.Add(character);
                return player;
            });

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
        var query = @"SELECT *
                      FROM ""Players"" AS p
                      LEFT JOIN ""Characters"" AS c on p.""Id"" = c.""PlayerId""
                      WHERE p.""Id"" = @Id";
        var queryParams = new { Id = id };

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
        var query = @"SELECT *
                      FROM ""Players""
                      WHERE ""Name"" = @Name";
        var queryParams = new { Name = name };

        using var connection = _context.CreateConnection();
        var player = await connection.QueryFirstOrDefaultAsync<Player>(new CommandDefinition(query, queryParams, cancellationToken: token));

        return player;
    }

    public async Task UpdateAsync(Player entity)
    {
        var query = @"UPDATE ""Players"" 
                      SET ""Name"" = @Name, ""Role"" = @Role,
                          ""PasswordHash"" = @PasswordHash,
                          ""PasswordSalt"" = @PasswordSalt,
                          ""RefreshToken"" = @RefreshToken,
                          ""RefreshTokenExpiryDate"" = @RefreshTokenExpiryDate
                      WHERE ""Id"" = @Id";
        var queryParams = new 
        {
            Name = entity.Name,
            Role = entity.Role,
            PasswordHash = entity.PasswordHash,
            PasswordSalt = entity.PasswordSalt,
            RefreshToken = entity.RefreshToken,
            RefreshTokenExpiryDate = entity.RefreshTokenExpiryDate,
            Id = entity.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }
}

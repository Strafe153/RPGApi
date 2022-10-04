using Core.Entities;
using Core.Enums;
using Core.Interfaces.Repositories;
using Core.Models;
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
        using var connection = _context.CreateConnection();
        string query = @"INSERT INTO ""Players"" (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
                         VALUES (@Name, @Role, @PasswordHash, @PasswordSalt)
                         RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Role = PlayerRole.Player,
            PasswordHash = entity.PasswordHash,
            PasswordSalt = entity.PasswordSalt
        };
        int id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        string query = @"DELETE FROM ""Players"" 
                         WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Player>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        using var connection = _context.CreateConnection();
        string query = @"SELECT * FROM ""Players"" p
                         LEFT OUTER JOIN ""Characters"" c on p.""Id"" = c.""PlayerId""
                         ORDER BY p.""Id"" ASC";

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
        using var connection = _context.CreateConnection();
        string query = @"SELECT * FROM ""Players"" p
                         LEFT OUTER JOIN ""Characters"" c on p.""Id"" = c.""PlayerId""
                         WHERE p.""Id"" = @Id";
        var queryParams = new { Id = id };

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
        using var connection = _context.CreateConnection();
        string query = @"SELECT * FROM ""Players""
                         WHERE ""Name"" = @Name";
        var queryParams = new { Name = name };

        var player = await connection.QueryFirstOrDefaultAsync<Player>(
            new CommandDefinition(query, queryParams, cancellationToken: token));

        return player;
    }

    public async Task UpdateAsync(Player entity)
    {
        using var connection = _context.CreateConnection();
        string query = @"UPDATE ""Players"" 
                         SET ""Name"" = @Name, ""Role"" = @Role, ""PasswordHash"" = @PasswordHash, ""PasswordSalt"" = @PasswordSalt
                         WHERE ""Id"" = @Id";
        var queryParams = new 
        {
            Name = entity.Name,
            Role = entity.Role,
            PasswordHash = entity.PasswordHash,
            PasswordSalt = entity.PasswordSalt,
            Id = entity.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }
}

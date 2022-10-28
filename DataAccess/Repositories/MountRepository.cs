using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Shared;
using Dapper;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace DataAccess.Repositories;

public class MountRepository : IItemRepository<Mount>
{
    private readonly RPGContext _context;

    public MountRepository(RPGContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Mount entity)
    {
        string query = @"INSERT INTO ""Mounts"" (""Name"", ""Type"", ""Speed"") 
                         VALUES (@Name, @Type, @Speed)
                         RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Speed = entity.Speed
        };

        using var connection = _context.CreateConnection();
        int id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        string query = @"DELETE FROM ""Mounts"" 
                         WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Mount>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        string mountsQuery = @"SELECT * FROM ""Mounts""
                         ORDER BY ""Id"" ASC";
        string characterMountsQuery = @"SELECT * FROM ""CharacterMounts"" cm
                                   LEFT OUTER JOIN ""Characters"" c on cm.""CharacterId"" = c.""Id""";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Mount>(new CommandDefinition(mountsQuery, cancellationToken: token));
        var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, token);

        var paginatedList = queryResult
            .GroupBy(m => m.Id)
            .Select(g =>
            {
                var m = g.First();
                m.CharacterMounts = characterMounts.Where(cm => cm.MountId == m.Id).ToList();

                return m;
            })
            .ToPaginatedList(pageNumber, pageSize);

        return paginatedList;
    }

    public async Task<Mount?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var queryParams = new { Id = id };
        string mountsQuery = @"SELECT * FROM ""Mounts""
                               WHERE ""Id"" = @Id";
        string characterMountsQuery = @$"SELECT * FROM ""CharacterMounts"" cm
                                        LEFT OUTER JOIN ""Characters"" c on cm.""CharacterId"" = c.""Id""
                                        WHERE cm.""MountId"" = {id}";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Mount>(
            new CommandDefinition(mountsQuery, queryParams, cancellationToken: token));
        var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, token);

        var mount = queryResult
            .GroupBy(m => m.Id)
            .Select(g =>
            {
                var m = g.First();
                m.CharacterMounts = characterMounts.ToList();

                return m;
            })
            .FirstOrDefault();

        return mount;
    }

    public async Task UpdateAsync(Mount entity)
    {
        string query = @"UPDATE ""Mounts"" 
                         SET ""Name"" = @Name, ""Type"" = @Type, ""Speed"" = @Speed
                         WHERE ""Id"" = @Id";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Speed = entity.Speed,
            Id = entity.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task AddToCharacterAsync(Character character, Mount item)
    {
        string query = @"INSERT INTO ""CharacterMounts"" (""CharacterId"", ""MountId"")
                         VALUES (@CharacterId, @MountId)";
        var queryParams = new
        {
            CharacterId = character.Id,
            MountId = item.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task RemoveFromCharacterAsync(Character character, Mount item)
    {
        string query = @"DELETE FROM ""CharacterMounts""
                         WHERE ""CharacterId"" = @CharacterId AND ""MountId"" = @MountId";
        var queryParams = new
        {
            CharacterId = character.Id,
            MountId = item.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    private async Task<IEnumerable<CharacterMount>> GetCharacterMountsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterMounts = await connection.QueryAsync<CharacterMount, Character, CharacterMount>(
            new CommandDefinition(query, cancellationToken: token),
            (characterMount, character) =>
            {
                characterMount.Character = character;
                return characterMount;
            });

        return characterMounts;
    }
}

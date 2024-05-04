using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Shared;
using Dapper;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class SpellRepository : IItemRepository<Spell>
{
    private readonly RPGContext _context;

    public SpellRepository(RPGContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Spell entity)
    {
        var queryParams = new
        {
            entity.Name,
            entity.Type,
            entity.Damage
        };

        var query = @"
            INSERT INTO ""Spells""
                (""Name"", ""Type"", ""Damage"") 
            VALUES
                (@Name, @Type, @Damage)
            RETURNING ""Id""";

        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public Task DeleteAsync(int id)
    {
        var queryParams = new { Id = id };
        var query = @"
            DELETE FROM ""Spells"" 
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var queryParams = new
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var query = @"
            SELECT s.*,
                   cs.*,
                   c.*
            FROM ""Spells"" AS s
            LEFT JOIN ""CharacterSpells"" AS cs ON s.""Id"" = cs.""SpellId""
            LEFT JOIN ""Characters"" AS c ON cs.""CharacterId"" = c.""Id""
            ORDER BY s.""Id"" ASC
            OFFSET @PageSize * (@PageNumber - 1)
            LIMIT @PageSize";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Spell, CharacterSpell, Character, Spell>(
            new CommandDefinition(query, queryParams, cancellationToken: token),
            (spell, characterSpell, character) =>
            {
                if (characterSpell is not null)
                {
                    characterSpell.Character = character;
                    spell.CharacterSpells.Add(characterSpell);
                }

                return spell;
            },
            splitOn: "Id, CharacterId, Id");

        return queryResult.ToPaginatedList(pageNumber, pageSize);
    }

    public async Task<Spell?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var queryParams = new { Id = id };
        var query = @"
            SELECT s.*,
                   cs.*,
                   c.*
            FROM ""Spells"" AS s
            LEFT JOIN ""CharacterSpells"" AS cs ON s.""Id"" = cs.""SpellId""
            LEFT JOIN ""Characters"" AS c ON cs.""CharacterId"" = c.""Id""
            WHERE s.""Id"" = @Id";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Spell, CharacterSpell, Character, Spell>(
            new CommandDefinition(query, queryParams, cancellationToken: token),
            (spell, characterSpell, character) =>
            {
                if (characterSpell is not null)
                {
                    characterSpell.Character = character;
                    spell.CharacterSpells.Add(characterSpell);
                }

                return spell;
            },
            splitOn: "Id, CharacterId, Id");

        return queryResult.FirstOrDefault();
    }

    public Task UpdateAsync(Spell entity)
    {
        var queryParams = new
        {
            entity.Name,
            entity.Type,
            entity.Damage,
            entity.Id
        };

        var query = @"
            UPDATE ""Spells"" 
            SET ""Name"" = @Name,
                ""Type"" = @Type,
                ""Damage"" = @Damage
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public Task AddToCharacterAsync(Character character, Spell item)
    {
        var queryParams = new
        {
            CharacterId = character.Id,
            SpellId = item.Id
        };

        var query = @"
            INSERT INTO ""CharacterSpells""
                (""CharacterId"", ""SpellId"")
            VALUES
                (@CharacterId, @SpellId)";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public Task RemoveFromCharacterAsync(Character character, Spell item)
    {
        var queryParams = new
        {
            CharacterId = character.Id,
            SpellId = item.Id
        };

        var query = @"
            DELETE FROM ""CharacterSpells""
            WHERE ""CharacterId"" = @CharacterId
                  AND ""SpellId"" = @SpellId";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }
}

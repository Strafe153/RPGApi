using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
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
        using var connection = _context.CreateConnection();
        string query = @"INSERT INTO ""Spells"" (""Name"", ""Type"", ""Damage"") 
                             VALUES (@Name, @Type, @Damage)
                             RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Damage = entity.Damage
        };
        int id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        string query = @"DELETE FROM ""Spells"" 
                         WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Spell>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        using var connection = _context.CreateConnection();
        string spellsQuery = @"SELECT * FROM ""Spells""
                               ORDER BY ""Id"" ASC";
        string characterSpellsQuery = @"SELECT * FROM ""CharacterSpells"" cs
                                        LEFT OUTER JOIN ""Characters"" c on cs.""CharacterId"" = c.""Id""";

        var spells = await connection.QueryAsync<Spell>(new CommandDefinition(spellsQuery, cancellationToken: token));
        var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, token);

        var paginatedList = spells
            .GroupBy(s => s.Id)
            .Select(g =>
            {
                var s = g.First();
                s.CharacterSpells = characterSpells.Where(cs => cs.SpellId == s.Id).ToList();

                return s;
            })
            .ToPaginatedList(pageNumber, pageSize);

        return paginatedList;
    }

    public async Task<Spell?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = _context.CreateConnection();
        var queryParams = new { Id = id };
        string spellsQuery = @"SELECT * FROM ""Spells""
                               WHERE ""Id"" = @Id";
        string characterSpellsQuery = @$"SELECT * FROM ""CharacterSpells"" cs
                                        LEFT OUTER JOIN ""Characters"" c on cs.""CharacterId"" = c.""Id""
                                        WHERE cs.""SpellId"" = {id}";

        var spells = await connection.QueryAsync<Spell>(
            new CommandDefinition(spellsQuery, queryParams, cancellationToken: token));
        var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, token);

        var spell = spells
            .GroupBy(s => s.Id)
            .Select(g =>
            {
                var s = g.First();
                s.CharacterSpells = characterSpells.ToList();

                return s;
            })
            .FirstOrDefault();

        return spell;
    }

    public async Task UpdateAsync(Spell entity)
    {
        using var connection = _context.CreateConnection();
        string query = @"UPDATE ""Spells"" 
                         SET ""Name"" = @Name, ""Type"" = @Type, ""Damage"" = @Damage
                         WHERE ""Id"" = @Id";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Damage = entity.Damage,
            Id = entity.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task AddToCharacterAsync(Character character, Spell item)
    {
        using var connection = _context.CreateConnection();
        string query = @"INSERT INTO ""CharacterSpells"" (""CharacterId"", ""SpellId"")
                         VALUES (@CharacterId, @SpellId)";
        var queryParams = new
        {
            CharacterId = character.Id,
            SpellId = item.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task RemoveFromCharacterAsync(Character character, Spell item)
    {
        using var connection = _context.CreateConnection();
        string query = @"DELETE FROM ""CharacterSpells""
                         WHERE ""CharacterId"" = @CharacterId AND ""SpellId"" = @SpellId";
        var queryParams = new
        {
            CharacterId = character.Id,
            SpellId = item.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }

    private async Task<IEnumerable<CharacterSpell>> GetCharacterSpellsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterSpells = await connection.QueryAsync<CharacterSpell, Character, CharacterSpell>(
            new CommandDefinition(query, cancellationToken: token),
            (characterSpell, character) =>
            {
                characterSpell.Character = character;
                return characterSpell;
            });

        return characterSpells;
    }
}

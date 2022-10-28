using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Shared;
using Dapper;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class CharacterRepository : IRepository<Character>
{
    private readonly RPGContext _context;

    public CharacterRepository(RPGContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Character entity)
    {
        string query = @"INSERT INTO ""Characters"" (""Name"", ""Race"", ""PlayerId"") 
                         VALUES (@Name, @Race, @PlayerId)
                         RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Race = entity.Race,
            PlayerId = entity.PlayerId
        };

        using var connection = _context.CreateConnection();
        int id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        string query = @"DELETE FROM ""Characters""
                         WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Character>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var charactersQuery = @"SELECT * FROM ""Characters""
                                ORDER BY ""Id"" ASC";
        var characterWeaponsQuery = @"SELECT * FROM ""CharacterWeapons"" cw
                                      LEFT OUTER JOIN ""Weapons"" w on cw.""WeaponId"" = w.""Id""";
        var characterSpellsQuery = @"SELECT * FROM ""CharacterSpells"" cs
                                     LEFT OUTER JOIN ""Spells"" s on cs.""SpellId"" = s.""Id""";
        var characterMountsQuery = @"SELECT * FROM ""CharacterMounts"" cm
                                     LEFT OUTER JOIN ""Mounts"" m on cm.""MountId"" = m.""Id""";

        using var connection = _context.CreateConnection();
        var characters = await connection.QueryAsync<Character>(new CommandDefinition(charactersQuery, cancellationToken: token));
        var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, token);
        var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, token);
        var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, token);

        var paginatedList = characters
            .GroupBy(c => c.Id)
            .Select(g =>
            {
                var c = g.First();
                c.Player = g.Select(c => c.Player).First();
                c.CharacterWeapons = characterWeapons.Where(cw => cw.CharacterId == c.Id).ToList();
                c.CharacterSpells = characterSpells.Where(cw => cw.CharacterId == c.Id).ToList();
                c.CharacterMounts = characterMounts.Where(cw => cw.CharacterId == c.Id).ToList();

                return c;
            })
            .ToPaginatedList(pageNumber, pageSize);

        return paginatedList;
    }

    public async Task<Character?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var queryParams = new { Id = id };
        var charactersQuery = @"SELECT * FROM ""Characters"" c
                                LEFT OUTER JOIN ""Players"" p on c.""PlayerId"" = p.""Id""
                                WHERE c.""Id"" = @Id";
        var characterWeaponsQuery = @$"SELECT * FROM ""CharacterWeapons"" cw
                                      LEFT OUTER JOIN ""Weapons"" w on cw.""WeaponId"" = w.""Id""
                                      WHERE cw.""CharacterId"" = {id}";
        var characterSpellsQuery = @$"SELECT * FROM ""CharacterSpells"" cs
                                     LEFT OUTER JOIN ""Spells"" s on cs.""SpellId"" = s.""Id""
                                     WHERE cs.""CharacterId"" = {id}";
        var characterMountsQuery = @$"SELECT * FROM ""CharacterMounts"" cm
                                     LEFT OUTER JOIN ""Mounts"" m on cm.""MountId"" = m.""Id""
                                     WHERE cm.""CharacterId"" = {id}";

        using var connection = _context.CreateConnection();
        var characters = await connection.QueryAsync<Character, Player, Character>(
            new CommandDefinition(charactersQuery, queryParams, cancellationToken: token), 
            (character, player) =>
            {
                character.Player = player;
                return character;
            });
        var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, token);
        var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, token);
        var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, token);

        var character = characters
            .GroupBy(c => c.Id)
            .Select(g =>
            {
                var c = g.First();
                c.Player = g.Select(c => c.Player).First();
                c.CharacterWeapons = characterWeapons.ToList();
                c.CharacterSpells = characterSpells.ToList();
                c.CharacterMounts = characterMounts.ToList();

                return c;
            })
            .FirstOrDefault();

        return character;
    }

    public async Task UpdateAsync(Character entity)
    {
        string query = @"UPDATE ""Characters"" 
                         SET ""Name"" = @Name, ""Race"" = @Race, ""Health"" = @Health
                         WHERE ""Id"" = @Id";
        var queryParams = new
        {
            Name = entity.Name,
            Race = entity.Race,
            Health = entity.Health,
            Id = entity.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    private async Task<IEnumerable<CharacterWeapon>> GetCharacterWeaponsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterWeapons = await connection.QueryAsync<CharacterWeapon, Weapon, CharacterWeapon>(
            new CommandDefinition(query, cancellationToken: token),
            (characterWeapon, weapon) =>
            {
                characterWeapon.Weapon = weapon;
                return characterWeapon;
            });

        return characterWeapons;
    }

    private async Task<IEnumerable<CharacterSpell>> GetCharacterSpellsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterSpels = await connection.QueryAsync<CharacterSpell, Spell, CharacterSpell>(
            new CommandDefinition(query, cancellationToken: token),
            (characterSpell, spell) =>
            {
                characterSpell.Spell = spell;
                return characterSpell;
            });

        return characterSpels;
    }

    private async Task<IEnumerable<CharacterMount>> GetCharacterMountsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterMounts = await connection.QueryAsync<CharacterMount, Mount, CharacterMount>(
            new CommandDefinition(query, cancellationToken: token),
            (characterMount, mount) =>
            {
                characterMount.Mount = mount;
                return characterMount;
            });

        return characterMounts;
    }
}

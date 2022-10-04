using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Models;
using Dapper;
using DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class WeaponRepository : IItemRepository<Weapon>
{
    private readonly RPGContext _context;

    public WeaponRepository(RPGContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Weapon entity)
    {
        using var connection = _context.CreateConnection();
        string query = @"INSERT INTO ""Weapons"" (""Name"", ""Type"", ""Damage"") 
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
        string query = @"DELETE FROM ""Weapons"" 
                         WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        using var connection = _context.CreateConnection();
        string weaponsQuery = @"SELECT * FROM ""Weapons""
                                ORDER BY ""Id"" ASC";
        string characterWeaponsQuery = @"SELECT * FROM ""CharacterWeapons"" cw
                                         LEFT OUTER JOIN ""Characters"" c on cw.""CharacterId"" = c.""Id""";

        var weapons = await connection.QueryAsync<Weapon>(new CommandDefinition(weaponsQuery, cancellationToken: token));
        var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, token);

        var paginatedList = weapons
            .GroupBy(w => w.Id)
            .Select(g =>
            {
                var w = g.First();
                w.CharacterWeapons = characterWeapons.Where(cw => cw.WeaponId == w.Id).ToList();

                return w;
            })
            .ToPaginatedList(pageNumber, pageSize);

        return paginatedList;
    }

    public async Task<Weapon?> GetByIdAsync(int id, CancellationToken token = default)
    {
        using var connection = _context.CreateConnection();
        var queryParams = new { Id = id };
        string weaponQuery = @"SELECT * FROM ""Weapons""
                          WHERE ""Id"" = @Id";
        string characterWeaponsQuery = @$"SELECT * FROM ""CharacterWeapons"" cw
                                         LEFT OUTER JOIN ""Characters"" c on cw.""CharacterId"" = c.""Id""
                                         WHERE cw.""WeaponId"" = {id}";

        var weapons = await connection.QueryAsync<Weapon>(
            new CommandDefinition(weaponQuery, queryParams, cancellationToken: token));
        var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, token);

        var weapon = weapons
            .GroupBy(w => w.Id)
            .Select(g =>
            {
                var w = g.First();
                w.CharacterWeapons = characterWeapons.ToList();

                return w;
            })
            .FirstOrDefault();

        return weapon;
    }

    public async Task UpdateAsync(Weapon entity)
    {
        using var connection = _context.CreateConnection();
        string query = @"UPDATE ""Weapons"" 
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

    public async Task AddToCharacterAsync(Character character, Weapon item)
    {
        using var connection = _context.CreateConnection();
        string query = @"INSERT INTO ""CharacterWeapons"" (""CharacterId"", ""WeaponId"")
                         VALUES (@CharacterId, @WeaponId)";
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task RemoveFromCharacterAsync(Character character, Weapon item)
    {
        using var connection = _context.CreateConnection();
        string query = @"DELETE FROM ""CharacterWeapons""
                         WHERE ""CharacterId"" = @CharacterId AND ""WeaponId"" = @WeaponId";
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        await connection.ExecuteAsync(query, queryParams);
    }

    private async Task<IEnumerable<CharacterWeapon>> GetCharacterWeaponsAsync(string query, CancellationToken token)
    {
        using var connection = _context.CreateConnection();
        var characterWeapons = await connection.QueryAsync<CharacterWeapon, Character, CharacterWeapon>(
            new CommandDefinition(query, cancellationToken: token),
            (characterWeapon, character) =>
            {
                characterWeapon.Character = character;
                return characterWeapon;
            });

        return characterWeapons;
    }
}

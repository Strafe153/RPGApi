using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Shared;
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
        var query = @"INSERT INTO ""Weapons"" (""Name"", ""Type"", ""Damage"") 
                      VALUES (@Name, @Type, @Damage)
                      RETURNING ""Id""";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Damage = entity.Damage
        };

        using var connection = _context.CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

        return id;
    }

    public async Task DeleteAsync(int id)
    {
        var query = @"DELETE FROM ""Weapons"" 
                      WHERE ""Id"" = @Id";
        var queryParams = new { Id = id };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var weaponsQuery = @"SELECT *
                             FROM ""Weapons""
                             ORDER BY ""Id"" ASC";
        var characterWeaponsQuery = @"SELECT *
                                      FROM ""CharacterWeapons"" AS cw
                                      LEFT JOIN ""Characters"" AS c on cw.""CharacterId"" = c.""Id""";

        using var connection = _context.CreateConnection();
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
        var queryParams = new { Id = id };
        var weaponQuery = @"SELECT *
                            FROM ""Weapons""
                            WHERE ""Id"" = @Id";
        var characterWeaponsQuery = @$"SELECT *
                                       FROM ""CharacterWeapons"" AS cw
                                       LEFT JOIN ""Characters"" AS c on cw.""CharacterId"" = c.""Id""
                                       WHERE cw.""WeaponId"" = {id}";

        using var connection = _context.CreateConnection();
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
        var query = @"UPDATE ""Weapons"" 
                      SET ""Name"" = @Name,
                          ""Type"" = @Type,
                          ""Damage"" = @Damage
                      WHERE ""Id"" = @Id";
        var queryParams = new
        {
            Name = entity.Name,
            Type = entity.Type,
            Damage = entity.Damage,
            Id = entity.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task AddToCharacterAsync(Character character, Weapon item)
    {
        var query = @"INSERT INTO ""CharacterWeapons"" (""CharacterId"", ""WeaponId"")
                      VALUES (@CharacterId, @WeaponId)";
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, queryParams);
    }

    public async Task RemoveFromCharacterAsync(Character character, Weapon item)
    {
        var query = @"DELETE FROM ""CharacterWeapons""
                      WHERE ""CharacterId"" = @CharacterId
                            AND ""WeaponId"" = @WeaponId";
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        using var connection = _context.CreateConnection();
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

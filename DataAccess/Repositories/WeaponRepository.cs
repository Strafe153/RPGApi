﻿using Core.Entities;
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
        var queryParams = new
        {
            entity.Name,
            entity.Type,
            entity.Damage
        };

        var query = @"
            INSERT INTO ""Weapons""
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
            DELETE FROM ""Weapons"" 
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public async Task<PaginatedList<Weapon>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default)
    {
        var queryParams = new
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var query = @"
            SELECT w.*,
                   cw.*,
                   c.*
            FROM ""Weapons"" AS w
            LEFT JOIN ""CharacterWeapons"" AS cw ON w.""Id"" = cw.""WeaponId""
            LEFT JOIN ""Characters"" AS c ON cw.""CharacterId"" = c.""Id""
            ORDER BY w.""Id"" ASC
            OFFSET @PageSize * (@PageNumber - 1)
            LIMIT @PageSize";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Weapon, CharacterWeapon, Character, Weapon>(
            new CommandDefinition(query, queryParams, cancellationToken: token),
            (weapon, characterWeapon, character) =>
            {
                if (characterWeapon is not null)
                {
                    characterWeapon.Character = character;
                    weapon.CharacterWeapons.Add(characterWeapon);
                }

                return weapon;
            },
            splitOn: "Id, CharacterId, Id");

        return queryResult.ToPaginatedList(pageNumber, pageSize);
    }

    public async Task<Weapon?> GetByIdAsync(int id, CancellationToken token = default)
    {
        var queryParams = new { Id = id };
        var query = @"
            SELECT w.*,
                   cw.*,
                   c.*
            FROM ""Weapons"" AS w
            LEFT JOIN ""CharacterWeapons"" AS cw ON w.""Id"" = cw.""WeaponId""
            LEFT JOIN ""Characters"" AS c ON cw.""CharacterId"" = c.""Id""
            WHERE w.""Id"" = @Id";

        using var connection = _context.CreateConnection();
        var queryResult = await connection.QueryAsync<Weapon, CharacterWeapon, Character, Weapon>(
            new CommandDefinition(query, queryParams, cancellationToken: token),
            (weapon, characterWeapon, character) =>
            {
                if (characterWeapon is not null)
                {
                    characterWeapon.Character = character;
                    weapon.CharacterWeapons.Add(characterWeapon);
                }

                return weapon;
            },
            splitOn: "Id, CharacterId, Id");

        return queryResult.FirstOrDefault();
    }

    public Task UpdateAsync(Weapon entity)
    {
        var queryParams = new
        {
            entity.Name,
            entity.Type,
            entity.Damage,
            entity.Id
        };

        var query = @"
            UPDATE ""Weapons"" 
            SET ""Name"" = @Name,
                ""Type"" = @Type,
                ""Damage"" = @Damage
            WHERE ""Id"" = @Id";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public Task AddToCharacterAsync(Character character, Weapon item)
    {
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        var query = @"
            INSERT INTO ""CharacterWeapons""
                (""CharacterId"", ""WeaponId"")
            VALUES
                (@CharacterId, @WeaponId)";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }

    public Task RemoveFromCharacterAsync(Character character, Weapon item)
    {
        var queryParams = new
        {
            CharacterId = character.Id,
            WeaponId = item.Id
        };

        var query = @"
            DELETE FROM ""CharacterWeapons""
            WHERE ""CharacterId"" = @CharacterId
                  AND ""WeaponId"" = @WeaponId";

        using var connection = _context.CreateConnection();
        return connection.ExecuteAsync(query, queryParams);
    }
}

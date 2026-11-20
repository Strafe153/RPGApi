using Dapper;
using DataAccess.Database;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class CharactersRepository : IRepository<Character>
{
	private readonly DatabaseConnectionProvider _connectionProvider;

	public CharactersRepository(DatabaseConnectionProvider connectionProvider)
	{
		_connectionProvider = connectionProvider;
	}

	public async Task<int> AddAsync(Character entity)
	{
		var queryParams = new
		{
			entity.Name,
			entity.Race,
			entity.PlayerId
		};

		var query = @"
            INSERT INTO Characters (""Name"", ""Race"", ""PlayerId"") 
            VALUES (@Name, @Race, @PlayerId)
            RETURNING ""Id""";

		using var connection = _connectionProvider.CreateConnection();
		var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

		return id;
	}

	public async Task DeleteAsync(int id)
	{
		var queryParams = new { Id = id };
		var query = @"
            DELETE FROM Characters
            WHERE ""Id"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	public async Task<GetAllResult<Character>> GetAllAsync(int pageNumber, int pageSize)
	{
		var query = @"
			SELECT COUNT(1)
			FROM Characters;

			SELECT *
            FROM Characters
            ORDER BY ""Id"" ASC;

			SELECT *
            FROM CharacterWeapons AS cw
            LEFT JOIN Weapons AS w on cw.""WeaponId"" = w.""Id"";

			SELECT *
            FROM CharacterSpells AS cs
            LEFT JOIN Spells AS s on cs.""SpellId"" = s.""Id"";

			SELECT *
            FROM CharacterMounts AS cm
            LEFT JOIN Mounts AS m on cm.""MountId"" = m.""Id"";";

		using var connection = _connectionProvider.CreateConnection();
		using var reader = await connection.QueryMultipleAsync(query);

		var count = await reader.ReadFirstAsync<int>();
		var characters = reader.Read<Character>();
		var characterWeapons = GetCharacterWeapons(reader);
		var characterSpells = GetCharacterSpells(reader);
		var characterMounts = GetCharacterMounts(reader);
			
		var items = characters
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
			.ToList();

		return new(count, items);
	}

	public async Task<Character?> GetByIdAsync(int id, CancellationToken token)
	{
		var queryParams = new { Id = id };

		var query = @"
			SELECT *
            FROM Characters AS c
            LEFT JOIN Players AS p on c.""PlayerId"" = p.""Id""
            WHERE c.""Id"" = @Id;

			SELECT *
            FROM CharacterWeapons AS cw
            LEFT JOIN Weapons AS w on cw.""WeaponId"" = w.""Id""
            WHERE cw.""CharacterId"" = @Id;

			SELECT *
            FROM CharacterSpells AS cs
            LEFT JOIN Spells AS s on cs.""SpellId"" = s.""Id""
            WHERE cs.""CharacterId"" = @Id;

			SELECT *
            FROM CharacterMounts AS cm
            LEFT JOIN Mounts AS m on cm.""MountId"" = m.""Id""
            WHERE cm.""CharacterId"" = @Id;";

		using var connection = _connectionProvider.CreateConnection();
		using var reader = await connection.QueryMultipleAsync(query, queryParams);

		var characters = reader.Read<Character, Player, Character>(
			(character, player) =>
			{
				character.Player = player;
				return character;
			});

		var characterWeapons = GetCharacterWeapons(reader);
		var characterSpells = GetCharacterSpells(reader);
		var characterMounts = GetCharacterMounts(reader);

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
		var queryParams = new
		{
			entity.Name,
			entity.Race,
			entity.Health,
			entity.Id
		};

		var query = @"
            UPDATE Characters
            SET ""Name"" = @Name,
                ""Race"" = @Race,
                ""Health"" = @Health
            WHERE ""Id"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	private static List<CharacterWeapon> GetCharacterWeapons(SqlMapper.GridReader reader)
	{
		var characterWeapons = reader
			.Read<CharacterWeapon, Weapon, CharacterWeapon>(
				(characterWeapon, weapon) =>
				{
					characterWeapon.Weapon = weapon;
					return characterWeapon;
				})
			.ToList();

		return characterWeapons;
	}

	private static List<CharacterSpell> GetCharacterSpells(SqlMapper.GridReader reader)
	{
		var characterSpells = reader
			.Read<CharacterSpell, Spell, CharacterSpell>(
				(characterSpell, spell) =>
				{
					characterSpell.Spell = spell;
					return characterSpell;
				})
			.ToList();

		return characterSpells;
	}

	private static List<CharacterMount> GetCharacterMounts(SqlMapper.GridReader reader)
	{
		var characterMounts = reader
			.Read<CharacterMount, Mount, CharacterMount>(
				(characterMount, mount) =>
				{
					characterMount.Mount = mount;
					return characterMount;
				})
			.ToList();

		return characterMounts;
	}
}

using Dapper;
using DataAccess.Database;
using Domain.Entities;
using Domain.Extensions;
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

	public async Task<PagedList<Character>> GetAllAsync(PageParameters pageParamters, CancellationToken token)
	{
		var charactersQuery = @"
            SELECT *
            FROM Characters
            ORDER BY ""Id"" ASC";
		var characterWeaponsQuery = @"
            SELECT *
            FROM CharacterWeapons AS cw
            LEFT JOIN Weapons AS w on cw.""WeaponId"" = w.""Id""";
		var characterSpellsQuery = @"
            SELECT *
            FROM CharacterSpells AS cs
            LEFT JOIN Spells AS s on cs.""SpellId"" = s.""Id""";
		var characterMountsQuery = @"
            SELECT *
            FROM CharacterMounts AS cm
            LEFT JOIN Mounts AS m on cm.""MountId"" = m.""Id""";

		using var connection = _connectionProvider.CreateConnection();
		var characters = await connection.QueryAsync<Character>(new CommandDefinition(charactersQuery, cancellationToken: token));
		var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, token: token);
		var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, token: token);
		var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, token: token);

		var pagedList = characters
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
			.ToPagedList(pageParamters);

		return pagedList;
	}

	public async Task<Character?> GetByIdAsync(int id, CancellationToken token)
	{
		var queryParams = new { Id = id };
		var charactersQuery = @"
            SELECT *
            FROM Characters AS c
            LEFT JOIN Players AS p on c.""PlayerId"" = p.""Id""
            WHERE c.""Id"" = @Id";
		var characterWeaponsQuery = @"
            SELECT *
            FROM CharacterWeapons AS cw
            LEFT JOIN Weapons AS w on cw.""WeaponId"" = w.""Id""
            WHERE cw.""CharacterId"" = @Id";
		var characterSpellsQuery = @"
            SELECT *
            FROM CharacterSpells AS cs
            LEFT JOIN Spells AS s on cs.""SpellId"" = s.""Id""
            WHERE cs.""CharacterId"" = @Id";
		var characterMountsQuery = @"
            SELECT *
            FROM CharacterMounts AS cm
            LEFT JOIN Mounts AS m on cm.""MountId"" = m.""Id""
            WHERE cm.""CharacterId"" = @Id";

		using var connection = _connectionProvider.CreateConnection();
		var characters = await connection.QueryAsync<Character, Player, Character>(
			new CommandDefinition(charactersQuery, queryParams, cancellationToken: token),
			(character, player) =>
			{
				character.Player = player;
				return character;
			});
		var characterWeapons = await GetCharacterWeaponsAsync(characterWeaponsQuery, queryParams, token);
		var characterSpells = await GetCharacterSpellsAsync(characterSpellsQuery, queryParams, token);
		var characterMounts = await GetCharacterMountsAsync(characterMountsQuery, queryParams, token);

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

	private async Task<IEnumerable<CharacterWeapon>> GetCharacterWeaponsAsync(
		string query,
		object? queryParams = default,
		CancellationToken token = default)
	{
		using var connection = _connectionProvider.CreateConnection();
		var characterWeapons = await connection.QueryAsync<CharacterWeapon, Weapon, CharacterWeapon>(
			queryParams is not null
				? new CommandDefinition(query, queryParams, cancellationToken: token)
				: new CommandDefinition(query, cancellationToken: token),
			(characterWeapon, weapon) =>
			{
				characterWeapon.Weapon = weapon;
				return characterWeapon;
			});

		return characterWeapons;
	}

	private async Task<IEnumerable<CharacterSpell>> GetCharacterSpellsAsync(
		string query,
		object? queryParams = default,
		CancellationToken token = default)
	{
		using var connection = _connectionProvider.CreateConnection();
		var characterSpels = await connection.QueryAsync<CharacterSpell, Spell, CharacterSpell>(
			queryParams is not null
				? new CommandDefinition(query, queryParams, cancellationToken: token)
				: new CommandDefinition(query, cancellationToken: token),
			(characterSpell, spell) =>
			{
				characterSpell.Spell = spell;
				return characterSpell;
			});

		return characterSpels;
	}

	private async Task<IEnumerable<CharacterMount>> GetCharacterMountsAsync(
		string query,
		object? queryParams = default,
		CancellationToken token = default)
	{
		using var connection = _connectionProvider.CreateConnection();
		var characterMounts = await connection.QueryAsync<CharacterMount, Mount, CharacterMount>(
			queryParams is not null
				? new CommandDefinition(query, queryParams, cancellationToken: token)
				: new CommandDefinition(query, cancellationToken: token),
			(characterMount, mount) =>
			{
				characterMount.Mount = mount;
				return characterMount;
			});

		return characterMounts;
	}
}

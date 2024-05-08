using Dapper;
using Domain.Entities;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class MountsRepository : IItemRepository<Mount>
{
	private readonly RPGContext _context;

	public MountsRepository(RPGContext context)
	{
		_context = context;
	}

	public async Task<int> AddAsync(Mount entity)
	{
		var queryParams = new
		{
			entity.Name,
			entity.Type,
			entity.Speed
		};

		var query = @"
            INSERT INTO ""Mounts""
                (""Name"", ""Type"", ""Speed"") 
            VALUES
                (@Name, @Type, @Speed)
            RETURNING ""Id""";

		using var connection = _context.CreateConnection();
		var id = await connection.ExecuteScalarAsync<int>(query, queryParams);

		return id;
	}

	public async Task DeleteAsync(int id)
	{
		var queryParams = new { Id = id };
		var query = @"
            DELETE FROM ""Mounts"" 
            WHERE ""Id"" = @Id";

		using var connection = _context.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	public async Task<PagedList<Mount>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
	{
		var queryParams = new
		{
			pageParameters.PageNumber,
			pageParameters.PageSize
		};

		var query = @"
            SELECT m.*,
                   cm.*,
                   c.*
            FROM ""Mounts"" AS m
            LEFT JOIN ""CharacterMounts"" AS cm ON m.""Id"" = cm.""MountId""
            LEFT JOIN ""Characters"" AS c ON cm.""CharacterId"" = c.""Id""
            ORDER BY m.""Id"" ASC
            OFFSET @PageSize * (@PageNumber - 1)
            LIMIT @PageSize";

		using var connection = _context.CreateConnection();
		var queryResult = await connection.QueryAsync<Mount, CharacterMount, Character, Mount>(
			new CommandDefinition(query, queryParams, cancellationToken: token),
			(mount, characterMount, character) =>
			{
				if (characterMount is not null)
				{
					characterMount.Character = character;
					mount.CharacterMounts.Add(characterMount);
				}

				return mount;
			},
			splitOn: "Id, CharacterId, Id");

		return queryResult.ToPagedList(pageParameters);
	}

	public async Task<Mount?> GetByIdAsync(int id, CancellationToken token)
	{
		var queryParams = new { Id = id };
		var query = @"
            SELECT m.*,
                   cm.*,
                   c.*
            FROM ""Mounts"" AS m
            LEFT JOIN ""CharacterMounts"" AS cm ON m.""Id"" = cm.""MountId""
            LEFT JOIN ""Characters"" AS c ON cm.""CharacterId"" = c.""Id""
            WHERE m.""Id"" = @Id";

		using var connection = _context.CreateConnection();
		var queryResult = await connection.QueryAsync<Mount, CharacterMount, Character, Mount>(
			new CommandDefinition(query, queryParams, cancellationToken: token),
			(mount, characterMount, character) =>
			{
				if (characterMount is not null)
				{
					characterMount.Character = character;
					mount.CharacterMounts.Add(characterMount);
				}

				return mount;
			},
			splitOn: "Id, CharacterId, Id");

		return queryResult.FirstOrDefault();
	}

	public async Task UpdateAsync(Mount entity)
	{
		var queryParams = new
		{
			entity.Name,
			entity.Type,
			entity.Speed,
			entity.Id
		};

		var query = @"
            UPDATE ""Mounts"" 
            SET ""Name"" = @Name,
                ""Type"" = @Type,
                ""Speed"" = @Speed
            WHERE ""Id"" = @Id";

		using var connection = _context.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	public async Task AddToCharacterAsync(Character character, Mount item)
	{
		var queryParams = new
		{
			CharacterId = character.Id,
			MountId = item.Id
		};

		var query = @"
            INSERT INTO ""CharacterMounts""
                (""CharacterId"", ""MountId"")
            VALUES
                (@CharacterId, @MountId)";

		using var connection = _context.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}

	public async Task RemoveFromCharacterAsync(Character character, Mount item)
	{
		var queryParams = new
		{
			CharacterId = character.Id,
			MountId = item.Id
		};

		var query = @"
            DELETE FROM ""CharacterMounts""
            WHERE ""CharacterId"" = @CharacterId
                  AND ""MountId"" = @MountId";

		using var connection = _context.CreateConnection();
		await connection.ExecuteAsync(query, queryParams);
	}
}

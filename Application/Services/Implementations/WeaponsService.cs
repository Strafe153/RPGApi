using Application.Mappers.Abstractions;
using Application.Services.Abstractions;
using DataAccess.Extensions;
using Domain.Dtos;
using Domain.Dtos.WeaponDtos;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class WeaponsService : IWeaponsService
{
	private readonly IItemRepository<Weapon> _repository;
	private readonly IMapper<Weapon, WeaponReadDto, WeaponCreateDto, WeaponUpdateDto> _mapper;
	private readonly ILogger<WeaponsService> _logger;

	public WeaponsService(
		IItemRepository<Weapon> repository,
		IMapper<Weapon, WeaponReadDto, WeaponCreateDto, WeaponUpdateDto> mapper,
		ILogger<WeaponsService> logger)
	{
		_repository = repository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<WeaponReadDto> AddAsync(WeaponCreateDto createDto)
	{
		var weapon = _mapper.Map(createDto);
		weapon.Id = await _repository.AddAsync(weapon);
		_logger.LogInformation("Succesfully created a weapon");

		return _mapper.Map(weapon);
	}

	public async Task DeleteAsync(int id, CancellationToken token)
	{
		await _repository.GetByIdOrThrowAsync(id, _logger, token);
		await _repository.DeleteAsync(id);

		_logger.LogInformation("Succesfully deleted a weapon with id {Id}", id);
	}

	public async Task<PageDto<WeaponReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
	{
		var pagedList = await _repository.GetAllAsync(pageParameters, token);
		_logger.LogInformation("Successfully retrieved all weapons");

		return _mapper.Map(pagedList);
	}

	public async Task<WeaponReadDto> GetByIdAsync(int id, CancellationToken token)
	{
		var weapon = await _repository.GetByIdOrThrowAsync(id, _logger, token);
		_logger.LogInformation("Successfully retrieved a weapon with id {Id}", id);

		return _mapper.Map(weapon);
	}

	public async Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<WeaponUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token)
	{
		var weapon = await _repository.GetByIdOrThrowAsync(id, _logger, token);
		var updateDto = _mapper.MapForPatch(weapon);

		patchDocument.ApplyTo(updateDto);

		if (!tryValidateModelDelegate(updateDto))
		{
			return false;
		}

		_mapper.Map(updateDto, weapon);
		await _repository.UpdateAsync(weapon);

		return true;
	}

	public async Task UpdateAsync(int id, WeaponUpdateDto updateDto, CancellationToken token)
	{
		var weapon = await _repository.GetByIdOrThrowAsync(id, _logger, token);
		_mapper.Map(updateDto, weapon);

		await _repository.UpdateAsync(weapon);
		_logger.LogInformation("Successfully updated a weapon with id {Id}", id);
	}
}

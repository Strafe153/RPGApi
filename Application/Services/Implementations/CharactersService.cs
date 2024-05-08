﻿using Application.Mappers.Abstractions;
using Application.Services.Abstractions;
using DataAccess.Extensions;
using Domain.Dtos;
using Domain.Dtos.CharacterDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class CharactersService : ICharactersService
{
	private readonly IRepository<Character> _charactersRepository;
	private readonly IPlayersRepository _playersRepository;
	private readonly IItemRepository<Weapon> _weaponsRepository;
	private readonly IItemRepository<Spell> _spellsRepository;
	private readonly IItemRepository<Mount> _mountsRepository;
	private readonly IAccessHelper _accessHelper;
	private readonly IMapper<Character, CharacterReadDto, CharacterCreateDto, CharacterUpdateDto> _mapper;
	private readonly ILogger<CharactersService> _logger;

	public CharactersService(
		IRepository<Character> charactersRepository,
		IPlayersRepository playersRepository,
		IItemRepository<Weapon> weaponsRepository,
		IItemRepository<Spell> spellsRepository,
		IItemRepository<Mount> mountsRepository,
		IAccessHelper accessHelper,
		IMapper<Character, CharacterReadDto, CharacterCreateDto, CharacterUpdateDto> mapper,
		ILogger<CharactersService> logger)
	{
		_charactersRepository = charactersRepository;
		_playersRepository = playersRepository;
		_weaponsRepository = weaponsRepository;
		_spellsRepository = spellsRepository;
		_mountsRepository = mountsRepository;
		_accessHelper = accessHelper;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<CharacterReadDto> AddAsync(CharacterCreateDto createDto, CancellationToken token)
	{
		var player = await _playersRepository.GetByIdOrThrowAsync(createDto.PlayerId, _logger, token);
		_accessHelper.VerifyAccessRights(player);

		var character = _mapper.Map(createDto);
		character.Id = await _charactersRepository.AddAsync(character);

		_logger.LogInformation("Succesfully created a character");

		return _mapper.Map(character);
	}

	public async Task DeleteAsync(int id, CancellationToken token)
	{
		var character = await _charactersRepository.GetByIdOrThrowAsync(id, _logger, token);
		_accessHelper.VerifyAccessRights(character.Player);

		await _charactersRepository.DeleteAsync(id);
		_logger.LogInformation("Succesfully deleted a character with id {Id}", id);
	}

	public async Task<PageDto<CharacterReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token)
	{
		var pagedList = await _charactersRepository.GetAllAsync(pageParameters, token);
		_logger.LogInformation("Successfully retrieved all characters");

		return _mapper.Map(pagedList);
	}

	public async Task<CharacterReadDto> GetByIdAsync(int id, CancellationToken token)
	{
		var character = await _charactersRepository.GetByIdOrThrowAsync(id, _logger, token);
		_logger.LogInformation("Successfully retrieved a character with id {Id}", id);

		return _mapper.Map(character);
	}

	public async Task UpdateAsync(int id, CharacterUpdateDto updateDto, CancellationToken token)
	{
		var character = await _charactersRepository.GetByIdOrThrowAsync(id, _logger, token);
		_accessHelper.VerifyAccessRights(character.Player);

		_mapper.Map(updateDto, character);
		await _charactersRepository.UpdateAsync(character);

		_logger.LogInformation("Successfully updated a character with id {Id}", id);
	}

	public async Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<CharacterUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token)
	{
		var character = await _charactersRepository.GetByIdOrThrowAsync(id, _logger, token);
		var updateDto = _mapper.MapForPatch(character);

		patchDocument.ApplyTo(updateDto);

		if (!tryValidateModelDelegate(updateDto))
		{
			return false;
		}

		_mapper.Map(updateDto, character);
		await _charactersRepository.UpdateAsync(character);

		return true;
	}

	public async Task ManageItemAsync(ManageItemDto itemDto, CancellationToken token)
	{
		var character = await _charactersRepository.GetByIdOrThrowAsync(itemDto.CharacterId, _logger, token);
		_accessHelper.VerifyAccessRights(character.Player);

		switch (itemDto.ItemType)
		{
			case ItemType.Weapon:
				await ManageWeaponAsync(itemDto, character, token);
				break;
			case ItemType.Spell:
				await ManageSpellAsync(itemDto, character, token);
				break;
			case ItemType.Mount:
				await ManageMountAsync(itemDto, character, token);
				break;
		}

		await _charactersRepository.UpdateAsync(character);
	}

	public async Task HitAsync(HitDto hitDto, CancellationToken token)
	{
		var dealer = await _charactersRepository.GetByIdOrThrowAsync(hitDto.DealerId, _logger, token);
		_accessHelper.VerifyAccessRights(dealer.Player);

		var damage = hitDto.Type switch
		{
			HitType.Weapon => (await _weaponsRepository.GetByIdOrThrowAsync(hitDto.ItemId, _logger, token)).Damage,
			_ => (await _spellsRepository.GetByIdOrThrowAsync(hitDto.ItemId, _logger, token)).Damage
		};

		var receiver = await _charactersRepository.GetByIdOrThrowAsync(hitDto.ReceiverId, _logger, token);

		CalculateHealth(receiver, damage);
		await _charactersRepository.UpdateAsync(receiver);
	}

	private async Task ManageWeaponAsync(ManageItemDto itemDto, Character character, CancellationToken token)
	{
		var weapon = await _weaponsRepository.GetByIdOrThrowAsync(itemDto.ItemId, _logger, token);

		switch (itemDto.Operation)
		{
			case ManageItemOperation.Add:
				await _weaponsRepository.AddToCharacterAsync(character, weapon);
				break;
			case ManageItemOperation.Remove:
				await _weaponsRepository.RemoveFromCharacterAsync(character, weapon);
				break;
		}
	}

	private async Task ManageSpellAsync(ManageItemDto itemDto, Character character, CancellationToken token)
	{
		var spell = await _spellsRepository.GetByIdOrThrowAsync(itemDto.ItemId, _logger, token);

		switch (itemDto.Operation)
		{
			case ManageItemOperation.Add:
				await _spellsRepository.AddToCharacterAsync(character, spell);
				break;
			case ManageItemOperation.Remove:
				await _spellsRepository.RemoveFromCharacterAsync(character, spell);
				break;
		}
	}

	private async Task ManageMountAsync(ManageItemDto itemDto, Character character, CancellationToken token)
	{
		var mount = await _mountsRepository.GetByIdOrThrowAsync(itemDto.ItemId, _logger, token);

		switch (itemDto.Operation)
		{
			case ManageItemOperation.Add:
				await _mountsRepository.AddToCharacterAsync(character, mount);
				break;
			case ManageItemOperation.Remove:
				await _mountsRepository.RemoveFromCharacterAsync(character, mount);
				break;
		}
	}

	private static void CalculateHealth(Character character, int damage)
	{
		if (character.Health - damage > 100)
		{
			character.Health = 100;
		}
		else if (character.Health < damage)
		{
			character.Health = 0;
		}
		else
		{
			character.Health -= damage;
		}
	}
}

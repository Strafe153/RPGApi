using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Application.Enums;
using Application.Extensions;
using Application.Interfaces;
using Application.Mappings;
using Application.Services.Abstractions;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class CharactersService(
    IRepository<Character> charactersRepository,
    IPlayersRepository playersRepository,
    IItemRepository<Weapon> weaponsRepository,
    IItemRepository<Spell> spellsRepository,
    IItemRepository<Mount> mountsRepository,
    IAccessChecker accessChecker,
    ILogger<CharactersService> logger) : ICharactersService
{
    public async Task<CharacterReadDto> AddAsync(CharacterCreateDto createDto, CancellationToken token)
    {
        var player = await playersRepository.GetByIdOrThrowAsync(createDto.PlayerId, logger, token);
        accessChecker.VerifyAccessRights(player);

        var character = createDto.ToCharacter();
        character.Id = await charactersRepository.AddAsync(character);

        logger.LogInformation("Succesfully created a character");

        var readDto = character.ToReadDto();

        return readDto;
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        var character = await charactersRepository.GetByIdOrThrowAsync(id, logger, token);
        accessChecker.VerifyAccessRights(character.Player);

        await charactersRepository.DeleteAsync(id);
        logger.LogInformation("Succesfully deleted a character with id {Id}", id);
    }

    public async Task<PageDto<CharacterReadDto>> GetAllAsync(PageParameters pageParameters)
    {
        var result = await charactersRepository.GetAllAsync(pageParameters.PageNumber, pageParameters.PageSize);
        var pageDto = result.ToPageDto(pageParameters);

        logger.LogInformation("Successfully retrieved all characters");

        return pageDto;
    }

    public async Task<CharacterReadDto> GetByIdAsync(int id, CancellationToken token)
    {
        var character = await charactersRepository.GetByIdOrThrowAsync(id, logger, token);
        logger.LogInformation("Successfully retrieved a character with id {Id}", id);

        var readDto = character.ToReadDto();

        return readDto;
    }

    public async Task UpdateAsync(int id, CharacterUpdateDto updateDto, CancellationToken token)
    {
        var character = await charactersRepository.GetByIdOrThrowAsync(id, logger, token);
        accessChecker.VerifyAccessRights(character.Player);

        updateDto.Update(character);
        await charactersRepository.UpdateAsync(character);

        logger.LogInformation("Successfully updated a character with id {Id}", id);
    }

    public async Task<bool> PatchAsync(
        int id,
        JsonPatchDocument<CharacterUpdateDto> patchDocument,
        Func<object, bool> tryValidateModelDelegate,
        CancellationToken token)
    {
        var character = await charactersRepository.GetByIdOrThrowAsync(id, logger, token);
        var updateDto = character.ToUpdateDto();

        patchDocument.ApplyTo(updateDto);

        if (!tryValidateModelDelegate(updateDto))
        {
            return false;
        }

        updateDto.Update(character);
        await charactersRepository.UpdateAsync(character);

        return true;
    }

    public async Task ManageItemAsync(ManageItemDto itemDto, CancellationToken token)
    {
        var character = await charactersRepository.GetByIdOrThrowAsync(itemDto.CharacterId, logger, token);
        accessChecker.VerifyAccessRights(character.Player);

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

        await charactersRepository.UpdateAsync(character);
    }

    public async Task HitAsync(HitDto hitDto, CancellationToken token)
    {
        var dealer = await charactersRepository.GetByIdOrThrowAsync(hitDto.DealerId, logger, token);
        accessChecker.VerifyAccessRights(dealer.Player);

        var damage = hitDto.Type switch
        {
            HitType.Weapon => await GetWeaponDamageAsync(hitDto, token),
            _ => await GetSpellDamageAsync(hitDto, token)
        };

        var receiver = await charactersRepository.GetByIdOrThrowAsync(hitDto.ReceiverId, logger, token);
        receiver.CalculateHealth(damage);

        await charactersRepository.UpdateAsync(receiver);
    }

    private async Task<int> GetWeaponDamageAsync(HitDto hitDto, CancellationToken token)
    {
        var weapon = await weaponsRepository.GetByIdOrThrowAsync(hitDto.ItemId, logger, token);
        return weapon.Damage;
    }

    private async Task<int> GetSpellDamageAsync(HitDto hitDto, CancellationToken token)
    {
        var spell = await spellsRepository.GetByIdOrThrowAsync(hitDto.ItemId, logger, token);
        return spell.Damage;
    }

    private async Task ManageWeaponAsync(ManageItemDto itemDto, Character character, CancellationToken token)
    {
        var weapon = await weaponsRepository.GetByIdOrThrowAsync(itemDto.ItemId, logger, token);

        switch (itemDto.Operation)
        {
            case ManageItemOperation.Add:
                await weaponsRepository.AddToCharacterAsync(character, weapon);
                break;
            case ManageItemOperation.Remove:
                await weaponsRepository.RemoveFromCharacterAsync(character, weapon);
                break;
        }
    }

    private async Task ManageSpellAsync(ManageItemDto itemDto, Character character, CancellationToken token)
    {
        var spell = await spellsRepository.GetByIdOrThrowAsync(itemDto.ItemId, logger, token);

        switch (itemDto.Operation)
        {
            case ManageItemOperation.Add:
                await spellsRepository.AddToCharacterAsync(character, spell);
                break;
            case ManageItemOperation.Remove:
                await spellsRepository.RemoveFromCharacterAsync(character, spell);
                break;
        }
    }

    private async Task ManageMountAsync(ManageItemDto itemDto, Character character, CancellationToken token)
    {
        var mount = await mountsRepository.GetByIdOrThrowAsync(itemDto.ItemId, logger, token);

        switch (itemDto.Operation)
        {
            case ManageItemOperation.Add:
                await mountsRepository.AddToCharacterAsync(character, mount);
                break;
            case ManageItemOperation.Remove:
                await mountsRepository.RemoveFromCharacterAsync(character, mount);
                break;
        }
    }
}

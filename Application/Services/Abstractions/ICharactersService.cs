using Application.Dtos;
using Application.Dtos.CharactersDtos;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Services.Abstractions;

public interface ICharactersService
{
	Task<PageDto<CharacterReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token);
	Task<CharacterReadDto> GetByIdAsync(int id, CancellationToken token);
	Task<CharacterReadDto> AddAsync(CharacterCreateDto createDto, CancellationToken token);
	Task UpdateAsync(int id, CharacterUpdateDto updateDto, CancellationToken token);
	Task DeleteAsync(int id, CancellationToken token);

	Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<CharacterUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token);

	Task ManageItemAsync(ManageItemDto itemDto, CancellationToken token);
	Task HitAsync(HitDto hitDto, CancellationToken token);
}

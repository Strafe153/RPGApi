using Application.Dtos;
using Application.Dtos.WeaponDtos;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Services.Abstractions;

public interface IWeaponsService
{
	Task<PageDto<WeaponReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token);
	Task<WeaponReadDto> GetByIdAsync(int id, CancellationToken token);
	Task<WeaponReadDto> AddAsync(WeaponCreateDto createDto);
	Task UpdateAsync(int id, WeaponUpdateDto updateDto, CancellationToken token);
	Task DeleteAsync(int id, CancellationToken token);

	Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<WeaponUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token);
}

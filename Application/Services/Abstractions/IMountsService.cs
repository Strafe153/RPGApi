using Domain.Dtos;
using Domain.Dtos.MountDtos;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Services.Abstractions;

public interface IMountsService
{
	Task<PageDto<MountReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token);
	Task<MountReadDto> GetByIdAsync(int id, CancellationToken token);
	Task<MountReadDto> AddAsync(MountCreateDto createDto);
	Task UpdateAsync(int id, MountUpdateDto updateDto, CancellationToken token);
	Task DeleteAsync(int id, CancellationToken token);

	Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<MountUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token);
}

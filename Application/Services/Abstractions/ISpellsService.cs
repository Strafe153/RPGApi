using Domain.Dtos;
using Domain.Dtos.SpellDtos;
using Domain.Shared;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Services.Abstractions;

public interface ISpellsService
{
	Task<PageDto<SpellReadDto>> GetAllAsync(PageParameters pageParameters, CancellationToken token);
	Task<SpellReadDto> GetByIdAsync(int id, CancellationToken token);
	Task<SpellReadDto> AddAsync(SpellCreateDto createDto);
	Task UpdateAsync(int id, SpellUpdateDto updateDto, CancellationToken token);
	Task DeleteAsync(int id, CancellationToken token);

	Task<bool> PatchAsync(
		int id,
		JsonPatchDocument<SpellUpdateDto> patchDocument,
		Func<object, bool> tryValidateModelDelegate,
		CancellationToken token);
}

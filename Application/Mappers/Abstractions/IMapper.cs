using Domain.Dtos;
using Domain.Shared;

namespace Application.Mappers.Abstractions;

public interface IMapper<TEntity, TReadDto, TCreateDto, TUpdateDto>
	where TEntity : class
	where TReadDto : class
	where TCreateDto : class
	where TUpdateDto : class
{
	PageDto<TReadDto> Map(PagedList<TEntity> list);
	TReadDto Map(TEntity entity);
	TEntity Map(TCreateDto dto);
	void Map(TUpdateDto dto, TEntity entity);
	TUpdateDto MapForPatch(TEntity entity);
}
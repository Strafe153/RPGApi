namespace Domain.Dtos;

public record PageDto<T>(
	int CurrentPage,
	int TotalPages,
	int PageSize,
	int TotalItems,
	bool HasPrevious,
	bool HasNext,
	IEnumerable<T> Entities);

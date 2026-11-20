namespace Application.Dtos;

public record PageDto<T>
{
    public required int PageSize { get; init; }
    public required int CurrentPage { get; init; }
    public required int TotalItems { get; init; }
    public required IEnumerable<T> Items { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
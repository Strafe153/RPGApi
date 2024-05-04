namespace Domain.Shared;

public class PaginatedList<T> : List<T>
{
    public int TotalItems { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginatedList(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize)
    {
        TotalItems = totalItems;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        AddRange(items);
    }
}

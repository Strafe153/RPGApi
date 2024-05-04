namespace Domain.Shared;

public class PageParameters
{
    private readonly int _maxPageSize = 20;
    private int _pageSize = 5;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }

        set
        {
            _pageSize = value > _maxPageSize ? _maxPageSize : value;
        }
    }

    public int PageNumber { get; set; } = 1;
}

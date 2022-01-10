namespace RPGApi.Dtos
{
    public record PageDto<T> where T : class
    {
        public IEnumerable<T>? Items { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
    }
}

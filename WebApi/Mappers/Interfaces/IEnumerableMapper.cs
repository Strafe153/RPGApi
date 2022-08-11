namespace WebApi.Mappers.Interfaces
{
    public interface IEnumerableMapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        public IEnumerable<TDestination> Map(IEnumerable<TSource> source)
        {
            return source.Select(i => Map(i));
        }
    }
}

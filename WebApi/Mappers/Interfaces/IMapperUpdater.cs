namespace WebApi.Mappers.Interfaces
{
    public interface IMapperUpdater<TFirst, TSecond>
    {
        void Map(TFirst first, TSecond second);
        TFirst Map(TSecond second);
    }
}

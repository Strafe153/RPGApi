namespace WebApi.Mappers.Interfaces
{
    public interface IUpdateMapper<TFirst, TSecond>
    {
        void Map(TFirst first, TSecond second);
        TFirst Map(TSecond second);
    }
}

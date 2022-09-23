namespace WebApi.Mappers.Interfaces;

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
}

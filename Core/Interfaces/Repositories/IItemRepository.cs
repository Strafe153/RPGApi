using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IItemRepository<T> : IRepository<T> where T : class
{
    Task AddToCharacterAsync(Character character, T item);
    Task RemoveFromCharacterAsync(Character character, T item);
}

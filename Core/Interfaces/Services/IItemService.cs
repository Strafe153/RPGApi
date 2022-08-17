using Core.Entities;

namespace Core.Interfaces.Services
{
    public interface IItemService<T> : IService<T> where T : class
    {
        void AddToCharacter(Character character, T item);
        void RemoveFromCharacter(Character character, T item);
    }
}

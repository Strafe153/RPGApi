using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByNameAsync(string name);
}

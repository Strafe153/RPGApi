using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByNameAsync(string name, CancellationToken token = default);
}

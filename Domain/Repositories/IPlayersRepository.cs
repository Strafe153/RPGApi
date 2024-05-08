using Domain.Entities;

namespace Domain.Repositories;

public interface IPlayersRepository : IRepository<Player>
{
    Task<Player?> GetByNameAsync(string name, CancellationToken token);
}

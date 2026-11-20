using Domain.Shared;

namespace Domain.Repositories;

public interface IRepository<T> where T : class
{
    Task<GetAllResult<T>> GetAllAsync(int pageNumber, int pageSize);
    Task<T?> GetByIdAsync(int id, CancellationToken token);
    Task<int> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

using Domain.Shared;

namespace Domain.Interfaces.Services;

public interface IService<T> where T : class
{
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default);
    Task<T> GetByIdAsync(int id, CancellationToken token = default);
    Task<int> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

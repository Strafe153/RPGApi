using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken token = default);
    Task<T?> GetByIdAsync(int id, CancellationToken token = default);
    Task SaveChangesAsync();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}

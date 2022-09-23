using Core.Models;

namespace Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize);
    Task<T?> GetByIdAsync(int id);
    Task SaveChangesAsync();
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}

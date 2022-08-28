using Core.Models;

namespace Core.Interfaces.Services
{
    public interface IService<T> where T : class
    {
        Task<PaginatedList<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}

namespace Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task SaveChangesAsync();
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

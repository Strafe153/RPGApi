namespace RPGApi.Repositories
{
    public interface IControllerRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void LogInformation(string message);
    }
}

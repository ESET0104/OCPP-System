using BackendAPI.Data.Entities.Users;

namespace BackendAPI.Repositories
{
    public interface IUserRepository<T> where T : IUser
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }

}

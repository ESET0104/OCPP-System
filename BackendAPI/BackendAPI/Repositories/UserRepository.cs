using BackendAPI.Data;
using BackendAPI.Data.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repositories
{
    public class UserRepository<T> : IUserRepository<T> where T : class, IUser
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

        public async Task<T> CreateAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }

}

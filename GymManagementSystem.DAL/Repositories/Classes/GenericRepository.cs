using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _context;
        public GenericRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAll(bool IsTracked, CancellationToken token = default)
        {
            var entities = IsTracked ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity?> GetById(int id, CancellationToken token = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return entity;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool IsTracked = false, CancellationToken token = default)
        {
            var items = IsTracked ? _context.Set<TEntity>() : _context.Set<TEntity>().AsNoTracking();
            return await items.FirstOrDefaultAsync(predicate, token);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, token);
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity != null)
                _context.Set<TEntity>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}

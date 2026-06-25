using GymManagementSystem.DAL.Entities;
using System.Linq.Expressions;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAll(bool IsTracked, CancellationToken token = default);
        Task<TEntity?> GetById(int id, CancellationToken token = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool IsTracked = false, CancellationToken token = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        Task<int> SaveChangesAsync();
    }
}

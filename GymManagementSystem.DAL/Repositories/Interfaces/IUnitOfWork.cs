using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        public ISessionRepository SessionRepository { get; }
        public Task<int> CompleteAsync();
    }
}

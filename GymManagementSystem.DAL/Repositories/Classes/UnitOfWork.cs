using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _Repos = [];

        public UnitOfWork(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            SessionRepository = new SessionRepository(_dbContext);
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            if(_Repos.TryGetValue(typeName, out var OldRepoditory))
            {
                return (IGenericRepository<TEntity>)OldRepoditory;
            }

            var newRepository = new GenericRepository<TEntity>(_dbContext);
            _Repos[typeName] = newRepository;
            return newRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}

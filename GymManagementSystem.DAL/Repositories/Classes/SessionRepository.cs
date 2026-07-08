using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext dbContext;

        public SessionRepository(GymDbContext _dbContext) : base(_dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken token)
        {
            var sessions = dbContext.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category);
            return await sessions.ToListAsync(token);
        }

        public async Task<Session> GetSessionByIdWithTrainerAndCategoryAsync(int SessionId, CancellationToken token)
        {
            var session = dbContext.Sessions.Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == SessionId);
            return await session;
        }

        public Task<int> GetCountOfBookedSlotAsync(int SessionId, CancellationToken token)
        {
            return dbContext.Bookings.CountAsync(b=>b.SessionId == SessionId, token);
        }
    }
}

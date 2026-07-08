using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        public Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken token);
        public Task<Session> GetSessionByIdWithTrainerAndCategoryAsync(int SessionId, CancellationToken token);
        public Task<int> GetCountOfBookedSlotAsync(int SessionId, CancellationToken token);
    }
}

using GymManagementSystem.DAL.Entities;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAll(bool IsTracked, CancellationToken token = default);
        Task<Plan?> GetById(int id, CancellationToken token = default);
        void Add (Plan plan);
        void Update(Plan plan);
        void Delete(int id);
        Task<int> SaveChangesAsync();
    }
}

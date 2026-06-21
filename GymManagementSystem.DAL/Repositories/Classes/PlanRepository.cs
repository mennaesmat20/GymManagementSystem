using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext _context;
        public PlanRepository(GymDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Plan>> GetAll(bool IsTracked, CancellationToken token = default)
        {
            var plans = IsTracked? _context.Plans : _context.Plans.AsNoTracking();
            return await _context.Plans.ToListAsync();
        }

        public async Task<Plan?> GetById(int id, CancellationToken token = default)
        {
            var plan = await _context.Plans.FindAsync(id);
            return plan;
        }

        public void Add(Plan plan)
        {
            _context.Plans.Add(plan);
        }

        public void Update(Plan plan)
        {
            _context.Plans.Update(plan);
        }

        public void Delete(int id)
        {
            var plan = _context.Plans.Find(id);
            if (plan != null)
                _context.Plans.Remove(plan);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
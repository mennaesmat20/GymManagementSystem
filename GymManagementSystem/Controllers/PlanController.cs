using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> _planRepository;
        public PlanController(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var plans = await _planRepository.GetAll(false, token);
            return View(plans);
        }

        public async Task<IActionResult> details(int id, CancellationToken token)
        {
            var plan = await _planRepository.GetById(id, token);
            if (plan == null)
                RedirectToAction(nameof(Index));
            return View(plan);
        }
    }
}

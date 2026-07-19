using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plan_ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanServices _planServices;
        public PlanController(IPlanServices planServices)
        {
            _planServices = planServices;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var plans = await _planServices.GetAllPlansAsync(token);
            return View(plans);
        }

        public async Task<IActionResult> details(int id, CancellationToken token)
        {
            var result = await _planServices.GetPlanDetailsAsync(id, token);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
                
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _planServices.GetPlanToUpdateAsync(id, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PlanToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _planServices.UpdatePlanDetailsAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update plan.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planServices.ToggleChangePlanStatusAsync(id, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Plan status updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update plan status.";

            return RedirectToAction(nameof(Index));
        }
    }
}

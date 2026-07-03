using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainer_ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerServices _trainerServices;
        public TrainerController(ITrainerServices trainerServices)
        {
            _trainerServices = trainerServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerServices.GetAllTrainersAsync(ct);
            return View(trainers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainer(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create), model);

            var result = await _trainerServices.CreateTrainerAsync(model, ct);
            if(result)
                TempData["Success"] = "Trainer created successfully.";
            else
                TempData["Failed"] = "Failed to create trainer.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> TrainerDetails(int id, CancellationToken ct)
        {
            var trainer = await _trainerServices.GetTrainerDetailsAsync(id, ct);
            if(trainer is null)
            {
                TempData["Failed"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> EditTrainer(int id, CancellationToken ct)
        {
            var trainer = await _trainerServices.GetTrainerToUpdateAsync(id, ct);
            if(trainer is null)
            {
                TempData["Failed"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainer(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _trainerServices.UpdateTrainerDetailsAsync(id, model, ct);
            if(result)
                TempData["Success"] = "Trainer updated successfully.";
            else
                TempData["Failed"] = "Failed to update trainer.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerServices.GetTrainerDetailsAsync(id, ct);
            if(trainer is null)
            {
                TempData["Failed"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
            var result = await _trainerServices.DeleteTrainerAsync(id, ct);
            if(result)
                TempData["Success"] = "Trainer deleted successfully.";
            else
                TempData["Failed"] = "Failed to delete trainer.";
            return RedirectToAction(nameof(Index));
        }
    }
}

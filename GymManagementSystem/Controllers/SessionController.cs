using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Session_ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionServices sessionServices;

        public SessionController(ISessionServices _sessionServices)
        {
            sessionServices = _sessionServices;
        }

        private async Task PopulateDropDownAsync(CancellationToken token)
        {
            ViewBag.Trainers = new SelectList(await sessionServices.GetTrainersForDropDownAsync(token), "Id", "Name");
            ViewBag.Categories = new SelectList(await sessionServices.GetCategoriesForDropDownAsync(token), "Id", "CategoryName");
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var sessions = await sessionServices.GetAllSessionsAsync(token);
            return View(sessions);
        }

        public async Task<IActionResult> Create(CancellationToken token)
        {
            await PopulateDropDownAsync(token);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken token)
        {
            if(!ModelState.IsValid)
            {
                await PopulateDropDownAsync(token);
                return View(model);
            }

            var result = await sessionServices.CreateSessionAsync(model, token);

            if(result.Success)
            {
                TempData["SuccessMessage"] = "Session created successfully!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.ErrorMessage;
            await PopulateDropDownAsync(token);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken token)
        {
            var session = await sessionServices.GetSessionByIdAsync(id, token);
            if(session == null)
            {
                TempData["ErrorMessage"] = "Session not found!";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken token)
        {
            var result = await sessionServices.GetSessionToUpdateAsync(id, token);
            if(!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDownAsync(token);
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken token)
        {
            if(!ModelState.IsValid)
            {
                await PopulateDropDownAsync(token);
                return View(model);
            }
            var result = await sessionServices.UpdateSessionAsync(id, model, token);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.ErrorMessage;

            await PopulateDropDownAsync(token);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken token)
        {
            var session = await sessionServices.GetSessionByIdAsync(id, token);
            if(session == null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken token)
        {
            var result = await sessionServices.RemoveSessionAsync(id,token);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Session Deleted Successfully" : result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }
    }
}

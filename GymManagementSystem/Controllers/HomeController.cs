using System.Diagnostics;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticsServices analyticsServices;

        public HomeController(IAnalyticsServices _analyticsServices)
        {
            analyticsServices = _analyticsServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var data = await analyticsServices.GetAnalyticsDataAsync(ct);
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

using System.Diagnostics;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
